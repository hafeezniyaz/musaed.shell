using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musaed.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoading = true;

        [ObservableProperty]
        private string _statusMessage = "Initializing...";

        [ObservableProperty]
        private string? _webViewSource;
    }
}
