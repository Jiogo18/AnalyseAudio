using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
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
