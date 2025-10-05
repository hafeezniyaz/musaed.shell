using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Musaed.Core.Dtos;
using Musaed.Core.Interfaces;
using Orchestrator.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Services
{
    public class OrchestratorApiClientService : IOrchestratorApiClient
    {
        private readonly ILogger<OrchestratorApiClientService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfigService _configService;
        public OrchestratorApiClientService(
            HttpClient httpClient,
            IConfigService configService,
            ILogger<OrchestratorApiClientService> logger)
        {
            _logger = logger;
            _configService = configService;
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadPackageAsync(Guid appId, string version, CancellationToken cancellationToken=default)
        {
            var relativePath = $"Apps/{appId}/Packages/{version}/download";

            var response = await _httpClient.GetAsync(
                relativePath, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
     
                response.EnsureSuccessStatusCode();

            await using var memoryStream = new MemoryStream();
            await response.Content.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);

            memoryStream.Position = 0;

            return memoryStream.ToArray();

        }


        public async Task<Guid> GetAppIdAsync(string appName)
        {
            var relativePath = $"Apps?name={appName}&isActive=true&skip=0&top=1";

            _logger.LogInformation($"fetching app details from {relativePath}");

            var response = await _httpClient.GetAsync(relativePath);
            response.EnsureSuccessStatusCode();

            var app = await response.Content.ReadFromJsonAsync<GetAppsResponseDto>();

            if (app == null || app.TotalCount == 0)
            {
                throw new NotFoundException($"requested app {appName} was not found or inactive");
            }

            return app.Items.First().Id;

        }

        public async Task<CredentialDto> GetCredentialAsync(Guid appId, string credentialName)
        {
            var relativePath = $"Apps/{appId}/Credentials/{credentialName}";

            _logger.LogInformation($"fetching credential details from {relativePath}");

            var response = await _httpClient.GetAsync(relativePath);
            response.EnsureSuccessStatusCode();

            var credential = await response.Content.ReadFromJsonAsync<CredentialDto>();
            return credential;
        }

        public async Task<PackageDto> GetPackageMetadataAsync(Guid appId, string version)
        {
            var relativePath = $"Apps/{appId}/Packages/{version}";

            _logger.LogInformation($"fetching asset details from {relativePath}");

            var response = await _httpClient.GetAsync(relativePath);
            response.EnsureSuccessStatusCode();

            var package = await response.Content.ReadFromJsonAsync<PackageDto>();

            return package;
        }

        public async Task<AppSettingsDto> GetRemoteSettingsAsync(Guid appId, string ConfigName, string assetName)
        {
            var relativePath = $"Apps/{appId}/Configs/{ConfigName}/Assets/{assetName}";

            _logger.LogInformation($"fetching asset details from {relativePath}");

            var response = await _httpClient.GetAsync(relativePath);
            response.EnsureSuccessStatusCode();


            var asset = await response.Content.ReadFromJsonAsync<ServerSettingsDto>();

            return new AppSettingsDto
            {
                ServerSettings = asset
            };

        }
    }
}
