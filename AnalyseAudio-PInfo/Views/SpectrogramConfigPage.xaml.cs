using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class SpectrogramConfigPage : Page
    {
        public SpectrogramConfigViewModel ViewModel { get; }

        public SpectrogramConfigPage()
        {
            ViewModel = App.GetService<SpectrogramConfigViewModel>();
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
