using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class SpectogramPage : Page
    {
        public SpectogramViewModel ViewModel { get; }

        public SpectogramPage()
        {
            ViewModel = App.GetService<SpectogramViewModel>();
            InitializeComponent();
        }
    }
}
