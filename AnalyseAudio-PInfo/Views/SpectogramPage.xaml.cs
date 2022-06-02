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
            DataContext = ViewModel;
            ViewModel.DispatcherQueue = DispatcherQueue;
            InitializeComponent();

            Unloaded += SpectrogramPage_Unloaded;
            Loading += SpectrogramPage_Loading;
        }

        ~SpectrogramPage()
        {
            ViewModel.Pause();
        }

        private void SpectrogramPage_Loading(Microsoft.UI.Xaml.FrameworkElement sender, object args)
        {
            ViewModel.Resume();
        }

        private void SpectrogramPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Pause();
        }
    }
}
