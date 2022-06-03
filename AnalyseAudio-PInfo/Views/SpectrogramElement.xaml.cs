using AnalyseAudio_PInfo.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class SpectrogramElement : Grid
    {
        public SpectrogramViewModel ViewModel { get; }

        public SpectrogramElement()
        {
            ViewModel = App.GetService<SpectrogramViewModel>();
            DataContext = ViewModel;
            InitializeComponent();

            Loading += SpectrogramElement_Loading;
            Unloaded += SpectrogramElement_Unloaded;
        }

        private void SpectrogramElement_Loading(Microsoft.UI.Xaml.FrameworkElement sender, object args)
        {
            ViewModel.Generator.ViewWantsSpectrogramImage(this);
        }

        private void SpectrogramElement_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Generator.ViewClosed(this);
        }
    }
}
