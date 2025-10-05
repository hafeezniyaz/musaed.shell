using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Musaed.App
{
    public class ExceptionHandlingService
    {
        private readonly ILogger<ExceptionHandlingService> _logger;

        public ExceptionHandlingService(ILogger<ExceptionHandlingService> logger)
        {
            _logger = logger;
        }

        public void Register()
        {
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(
            object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e
            )
        {
            _logger.LogCritical(e.Exception, "An unhandled exception occurred");

            e.Handled = true;

            MessageBox.Show(
                e.Exception.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );

            Application.Current.Shutdown();
        }
    }
}