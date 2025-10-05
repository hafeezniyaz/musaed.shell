using Microsoft.Extensions.Logging;
using Musaed.App.ViewModels;
using Musaed.Core.Common;
using Musaed.Core.Dtos;
using Musaed.Core.Interfaces;
using Musaed.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.App
{
    public class Bootstrapper
    {
        private readonly ILogger<Bootstrapper> _logger;
        private readonly MainViewModel _viewModel;
        private readonly IOrchestratorApiClient _apiCLient;
        private readonly IConfigService _config;
        private readonly IAuthService _auth;
        private readonly ICacheService _cache;
        private readonly IProcessManager _processManager;
        private readonly IPackageManager _packageManager;
       

        public Bootstrapper(
            ILogger<Bootstrapper> logger, 
            MainViewModel viewModel,
            IOrchestratorApiClient apiClient,
            IConfigService config,
            IAuthService auth,
            IProcessManager processManager,
            IPackageManager packageManager,
            ICacheService cache
            )
        {
            _logger = logger;
            _viewModel = viewModel;
            _apiCLient = apiClient;
            _auth = auth;
            _cache = cache;
            _packageManager = packageManager;
            _processManager = processManager;
            _config = config;
        }

        public async Task<AppSettingsDto> RunAsync()
        {
            _logger.LogInformation("Initializng the app...");
            AppSettingsDto appSettings = null;
            Guid appId = Guid.Empty;
            try
            {

                //auth
                _viewModel.StatusMessage = "Authenticating...";
                await _auth.GetAccessTokenAsync();

                //get app id
                _viewModel.StatusMessage = "Fetching application configuration...";
                var appName = _config.Config.AppName;
                appId = await _apiCLient.GetAppIdAsync(appName);

                //get appsettings
                appSettings = await _apiCLient.GetRemoteSettingsAsync
                    (
                    appId, _config.Config.ConfigName, _config.Config.AssetName
                    );
                _logger.LogInformation("successfully fetched setting from server");
                _cache.Save(appSettings, Constants.AppSettings.AppsettingsCacheKey);



            }
            catch(AuthenticationException ex)
            {
                _logger.LogError(ex, "Authentication failed");
                _viewModel.StatusMessage = "Authentication Error." +
                   " Please check your network or contact ITHelpDesk";
                return null;
            }
            catch(Exception ex)
            {
                var errorMessage = " Could not connect to server. Attempting to lauch in offline mode...";
                _logger.LogError(ex, errorMessage);
                _viewModel.StatusMessage = $"{errorMessage}" +
                    $" please check the log for details or contact ITHelpDesk";
                await Task.Delay(2000);
                appSettings = _cache.Load<AppSettingsDto>(Constants.AppSettings.AppsettingsCacheKey);
            }

            if(appSettings == null)
            {
                _logger.LogCritical("Failed to start the application . " +
                    "Could not fetch settings from server or local");
                _viewModel.StatusMessage = "Attempt to lauch in offline mode failed." +
                    " Please check you network or contact ITHelpDesk";
                await Task.Delay(3000);
                return null;
            }

            _logger.LogInformation($"Using settings for package version " +
                $"{appSettings.ServerSettings.Version}");
            try
            {

                _viewModel.StatusMessage = "Perparing application...";
               // await _packageManager.EnsurePackageIsReadyAsync(appId, appSettings.ServerSettings);


                _viewModel.StatusMessage = "We are almost there...";

               //await _processManager.StartServerAsync(appSettings, appId);

                _viewModel.StatusMessage = "Loading the application...";


                return appSettings;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A critical error occurred during package or process startup.");
                _viewModel.StatusMessage = $"Error: {ex.Message}";
                await Task.Delay(5000); 
                return null;
            }



        }
    }
}
