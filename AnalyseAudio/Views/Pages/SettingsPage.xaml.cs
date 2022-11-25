using AnalyseAudio.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            ViewModel = App.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}
