using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class InputPage : Page
    {
        public InputViewModel ViewModel { get; }

        public InputPage()
        {
            ViewModel = App.GetService<InputViewModel>();
            InitializeComponent();
        }

    }
}
