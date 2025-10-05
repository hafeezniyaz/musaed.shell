using Microsoft.Extensions.Logging;
using Musaed.Core.Common;
using Musaed.Core.Dtos;
using Musaed.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Security;
using System.Numerics;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.Infrastructure.Services
{
    public class AuthService : IAuthService

    {
        private readonly ILogger<AuthService> _logger;
        private readonly IConfigService _configService;
        private readonly HttpClient _httpClient;

        private string? _cachedAccessToken;
        private DateTime _tokenExpiryTime = DateTime.Now;

        public AuthService(
            ILogger<AuthService> logger,
            IConfigService configService,
            HttpClient httpClient
            )
        {
            _logger = logger;
            _configService = configService;
            _httpClient = httpClient;

        }

        public async Task<string> GetAccessTokenAsync()
        {
            var authMode = _configService.Config.AuthMode;

            if ("windows".Equals(authMode, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if(!string.IsNullOrEmpty(_cachedAccessToken) && DateTime.UtcNow < _tokenExpiryTime)
            {
                return _cachedAccessToken;
            }

            try
            {
                var tokenEndpoint = new Uri(new Uri(_configService.Config.OrchestratorUrl),
                    Constants.Authorization.AuthTokenApiRelativePath);

                if (string.IsNullOrEmpty(_configService.Config.ClientId) || 
                    string.IsNullOrEmpty(_configService.Config.ClientSecret))
                    throw new  AuthenticationException("Auth mode : OAuth needs " +
                        "clientId and client secret to added to config.json file");


                var requestBody = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type",Constants.Authorization.granType),
                    new KeyValuePair<string,string>("client_id",_configService.Config.ClientId),
                    new KeyValuePair<string,string>("client_secret",_configService.Config.ClientSecret),

                });

                var response = await _httpClient.PostAsync(tokenEndpoint, requestBody);
                response.EnsureSuccessStatusCode();

                var tokenResonse = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

                if (string.IsNullOrEmpty(tokenResonse?.AccessToken))
                {
                    throw new AuthenticationException("invalid authentication response from the server");
                }

                _cachedAccessToken = tokenResonse.AccessToken;
                _tokenExpiryTime = DateTime.UtcNow.AddSeconds(tokenResonse.ExpiresInSeconds - 60);

                return _cachedAccessToken;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed retrieve access token from the server");
                throw new AuthenticationException("Authentication failed :" + ex);
            }
        }
    }
}
