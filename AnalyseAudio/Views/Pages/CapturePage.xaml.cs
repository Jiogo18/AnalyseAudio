using AnalyseAudio.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio.Views
{
    public sealed partial class CapturePage : Page
    {
        public CaptureViewModel ViewModel { get; }

        public CapturePage()
        {
            ViewModel = App.GetService<CaptureViewModel>();
            InitializeComponent();
        }
    }
}
