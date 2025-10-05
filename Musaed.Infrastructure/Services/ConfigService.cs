using Musaed.Core.Common;
using Musaed.Core.Common.Exceptions;
using Musaed.Core.Dtos;
using Musaed.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Services
{
    public class ConfigService : IConfigService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true      
        };

        public AppConfig Config { get; }

        public ConfigService()
        {
            try
            {
                var configPath = Path.Combine(AppContext.BaseDirectory, Constants.Configuartion.ConfigFileName);

                if (!File.Exists(configPath))
                    throw new ConfigurationException("Config.json file was not found in the app root directory");

                var json = File.ReadAllText(configPath);

                Config = JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions);

                if (Config is null)
                    throw new JsonException("no data found in config.json file");
            }
            catch(ConfigurationException ex)
            {
                throw new ConfigurationException(ex.Message);
            }



        }
    }
}
