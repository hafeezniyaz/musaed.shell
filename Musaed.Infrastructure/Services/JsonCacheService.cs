using Musaed.Core.Common;
using Musaed.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Services
{
    public class JsonCacheService : ICacheService
    {
        private readonly string _cacheDirectory;

        public JsonCacheService(IConfigService configService)
        {
            var rootPath = Environment.ExpandEnvironmentVariables(configService.Config.PackageDownloadPath);
            _cacheDirectory = Path.Combine(rootPath, Constants.Configuartion.CacheFolderName);
            Directory.CreateDirectory(_cacheDirectory);
        }

        public T Load<T>(string key)
        {
            var filePath = Path.Combine(_cacheDirectory, $"{key}.json");
            if (!File.Exists(filePath))
            {
                return default;
            }

            var json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }


        public void Save<T>(T data, string key)
        {
            var filePath = Path.Combine(_cacheDirectory, $"{key}.json");
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
}
