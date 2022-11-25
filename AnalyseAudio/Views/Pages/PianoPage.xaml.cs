using AnalyseAudio.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio.Views
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
