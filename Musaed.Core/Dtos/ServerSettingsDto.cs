using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Musaed.Core.Dtos
{
    public class ServerSettingsDto
    {
        [JsonPropertyName("appName")]
        public required string AppName { get; set; }

        [JsonPropertyName("version")]
        public required string Version { get; set; }

        [JsonPropertyName("url")]
        public string?  Url { get; set; }

        [JsonPropertyName("continueInBackgroundEnabled")]
        public bool? ContinueInBackgroundEnabled { get; set; } = false;

        [JsonPropertyName("lanuchInBackgroundOnlyEnabled")]
        public bool? LanuchInBackgroundOnlyEnabled { get; set; } = false;

        [JsonPropertyName("exeName")]
        public required string ExeName { get; set; }

        [JsonPropertyName("healthApiPath")]
        public string? HealthApiPath { get; set; } = "/health";

        [JsonPropertyName("useWindowsAuth")]
        public bool? UseWindowsAuth { get; set; } = false;
    }

}
