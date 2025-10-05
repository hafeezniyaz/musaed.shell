using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Musaed.Core.Dtos
{
    public class AppConfig
    {
        [JsonPropertyName("orchestratorUrl")]
        public required string OrchestratorUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("appName")]
        public  required string AppName { get; set; }

        [JsonPropertyName("configName")]
        public required string ConfigName { get; set; } = string.Empty;

        [JsonPropertyName("assetName")]
        public required string AssetName { get; set; } = string.Empty;

        [JsonPropertyName("authMode")]
        public string AuthMode  { get; set; } = "windows";

        [JsonPropertyName("clientId")]
        public string? ClientId { get; set; }

        [JsonPropertyName("clientSecret")]
        public string? ClientSecret { get; set; }

        [JsonPropertyName("packageDownloadPath")]
        public string? PackageDownloadPath { get; set; }

        [JsonPropertyName("logToOrchestrator")]
        public bool LogToOrchestrator { get; set; } = true;

        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; } = "v1";

        [JsonPropertyName("shellName")]
        public string ShellName { get; set; } = "Musaed.App";
    }
}
