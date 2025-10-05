using System;
using System.Collections.Generic;
using System.Windows;


namespace Musaed.App
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>

    public partial class BrowserWindow : Window
    {
        private bool _isWebViewInitialized = false;
        public BrowserWindow()
        {
            InitializeComponent();
        }

        public async Task NavigateAsync(string url)
        {
           
            if (!_isWebViewInitialized)
            {
                try
                {
                    await WebView.EnsureCoreWebView2Async(null);
                    _isWebViewInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to initialize. Error: {ex.Message}", "WebView2 Error");
                    return;
                }
            }

            
            if (WebView != null && WebView.CoreWebView2 != null)
            {
                WebView.CoreWebView2.Navigate(url);
            }
        }
    }
}
