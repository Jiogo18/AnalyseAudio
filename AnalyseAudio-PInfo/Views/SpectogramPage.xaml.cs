using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class SpectrogramPage : Page
    {
        public SpectrogramViewModel ViewModel { get; }

        public SpectrogramPage()
        {
            ViewModel = App.GetService<SpectrogramViewModel>();
            InitializeComponent();
        }
    }
}
