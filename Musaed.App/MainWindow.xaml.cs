using Musaed.App.ViewModels;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Musaed.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
        this.MouseLeftButtonDown += (s, e) => { if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed) this.DragMove(); };
        this.Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await WebView.EnsureCoreWebView2Async(null);
            // Subscribe to the NavigationCompleted event.
            WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize WebView2. Please ensure the WebView2 Runtime is installed. Error: {ex.Message}", "WebView2 Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (this.DataContext is MainViewModel vm)
            {
                vm.IsLoading = false;
            }
        });
    }
}