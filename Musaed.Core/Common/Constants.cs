using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Core.Common
{
    public static class Constants
    {
        public static class Configuartion
        {
            public const string ConfigFileName = "config.json";
            public const string CacheFolderName = "Cache";

        }

        public static class Authorization
        {
            public const string AuthTokenApiRelativePath = "connect/token";
            public const string granType = "client_credentials";
            public const string WindowsAuthMode = "windows";
        }

        public static class AppSettings
        {
            public const string AppsettingsCacheKey = "appsettings.cache";
        }

        public static class SerlogSettings
        {
            public const string LoggingClientName = "LoggingClient";
            public const string LogBufferFileName = "log-buffer.json";
            public const string LogFolderName = "Logs";
            public const string LogFileNamePattern = "log-.json";        }


    }
}
