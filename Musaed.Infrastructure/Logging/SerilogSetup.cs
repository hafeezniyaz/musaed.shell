using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Musaed.Core.Common;
using Musaed.Core.Interfaces;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Logging
{
    public static class SerilogSetup
    {
        public static void Configure(IServiceProvider services, LoggerConfiguration loggerConfiguration) 
        {
            var configService = services.GetRequiredService<IConfigService>();
            var appConfig = configService.Config;
            var appName = appConfig.ShellName;

            var logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                appName,
                Constants.SerlogSettings.LogFolderName);

            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(
                logDirectory,
                Constants.SerlogSettings.LogFileNamePattern);



            loggerConfiguration
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.File(
            new JsonFormatter(),
            logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 10);


            if(appConfig is { LogToOrchestrator:true,OrchestratorUrl: not null })
            {
                var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

               // var logApiEndpoint = new Uri(new Uri(appConfig.OrchestratorUrl), "v1/api/logs");
                var bufferFilePath = Path.Combine(logDirectory, Constants.SerlogSettings.LogBufferFileName);

                var httpSink = new HttpSink(appName, bufferFilePath, httpClientFactory,Constants.SerlogSettings.LoggingClientName);

                var batchingOptions = new PeriodicBatchingSinkOptions
                {
                    BatchSizeLimit = 50,
                    Period = TimeSpan.FromSeconds(5),
                    EagerlyEmitFirstEvent = true
                };

                var bacthingSink = new PeriodicBatchingSink(httpSink, batchingOptions);
                loggerConfiguration.WriteTo.Sink(bacthingSink);
            }

        }
    }
} 
