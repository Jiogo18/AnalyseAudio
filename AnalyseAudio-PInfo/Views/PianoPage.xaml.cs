using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class PianoPage : Page
    {
        public PianoViewModel ViewModel { get; }

        public PianoPage()
        {
            ViewModel = App.GetService<PianoViewModel>();
            InitializeComponent();
        }
    }
}
