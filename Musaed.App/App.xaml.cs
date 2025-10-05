using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Musaed.App.ViewModels;
using Musaed.Core.Common;
using Musaed.Core.Dtos;
using Musaed.Core.Interfaces;
using Musaed.Infrastructure.Handlers;
using Musaed.Infrastructure.Logging;
using Musaed.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Core;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace Musaed.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;


    public App()
    {
        _host = Host.CreateDefaultBuilder()

            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfigService, ConfigService>();

                services.AddTransient<AuthenticationHandler>();

                //Auth Client
                services.AddHttpClient<IAuthService, AuthService>((serviceProvider, client) =>
                {
                    var configService = serviceProvider.GetRequiredService<IConfigService>();
                    client.BaseAddress = new Uri(configService.Config.OrchestratorUrl);
                });


                //Handler for auth types
                Func<IServiceProvider, HttpClientHandler> primaryHandlerFactory = sp =>
                {
                    var configService = sp.GetRequiredService<IConfigService>();
                    var handler = new HttpClientHandler();
                    if (Core.Common.Constants.Authorization.WindowsAuthMode.Equals(configService.Config.AuthMode,
                        StringComparison.OrdinalIgnoreCase))
                        handler.UseDefaultCredentials = true;
                    return handler;

                };

                //policy
                var retryPolicy = (IServiceProvider sp, HttpRequestMessage request) =>
                {
                    var logger = sp.GetRequiredService<ILogger<App>>();

                    return HttpPolicyExtensions
                    .HandleTransientHttpError()
                   .WaitAndRetryAsync
                       (
                           3,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            onRetry: (outcome, timespan, retyAttempt, context) =>
                            {
                                logger.LogWarning(
                                    $"Request to url {request.RequestUri} failed with status code " +
                                    $"{outcome.Result?.StatusCode}. " +
                                    $"waiting {timespan.TotalSeconds} before next retry. " +
                                    $"Retry attempt {retyAttempt}"
                                    );
                            }
                       );

                };

                //api client config
                Action<IServiceProvider, HttpClient> configureClient = (sp, client) =>
                {
                    var configService = sp.GetRequiredService<IConfigService>();
                    client.BaseAddress = new Uri(
                        $"{configService.Config.OrchestratorUrl}/api/{configService.Config.ApiVersion}/");
                };


                //orchestrator api client
                services.AddHttpClient<IOrchestratorApiClient,
                    OrchestratorApiClientService>(configureClient)
                .ConfigurePrimaryHttpMessageHandler(primaryHandlerFactory)
                .AddHttpMessageHandler<AuthenticationHandler>()
                .AddPolicyHandler(retryPolicy);

                //logging api client 
                services.AddHttpClient("LoggingClient", configureClient)
                .ConfigurePrimaryHttpMessageHandler(primaryHandlerFactory)
                .AddHttpMessageHandler<AuthenticationHandler>()
                .AddPolicyHandler(retryPolicy);


                services.AddSingleton<ICacheService, JsonCacheService>();
                services.AddSingleton<IPackageManager, PackageManagerService>();
                services.AddSingleton<IProcessManager, ProcessManagerService>();
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();
                services.AddTransient<BrowserWindow>();
                services.AddSingleton<Bootstrapper>();
                services.AddSingleton<ExceptionHandlingService>();

            })
            .UseSerilog((context, services, loggerConfiguration) =>
            {
                SerilogSetup.Configure(services, loggerConfiguration);
            })
            .Build();
    }


    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var exceptionHandler = _host.Services.GetRequiredService<ExceptionHandlingService>();
        exceptionHandler.Register();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        var bootstrapper = _host.Services.GetRequiredService<Bootstrapper>();
        var appSettings = await bootstrapper.RunAsync();

        if (appSettings != null)
        {
            var browserWindow = _host.Services.GetRequiredService<BrowserWindow>();
            browserWindow.Title = appSettings.ServerSettings.AppName;
            Application.Current.MainWindow = browserWindow;

            if (appSettings.ServerSettings.LanuchInBackgroundOnlyEnabled == false)
                browserWindow.Show();

            mainWindow.Close();

            await browserWindow.NavigateAsync(appSettings.ServerSettings.Url);

        }
        else
        {
            MessageBox.Show("Failed to start the application. Please check logs for details.",
                "Startup Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        var processManager = _host.Services.GetRequiredService<IProcessManager>();
        try
        {

            await processManager.StopServer();

        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error occurred while trying to close the local server");

        }
        finally
        {
            using (_host)
            {

                await _host.StopAsync(TimeSpan.FromSeconds(5));

            }
            base.OnExit(e);
        }


    }


}

