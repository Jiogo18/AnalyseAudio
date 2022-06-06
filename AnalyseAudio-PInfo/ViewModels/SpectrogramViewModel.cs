using AnalyseAudio_PInfo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class SpectrogramViewModel : ObservableRecipient
    {
        public readonly SpectrogramGenerator Generator;
        BitmapImage _spectrogram = new();
        public BitmapImage SpectrogramImage
        {
            get => _spectrogram;
            private set
            {
                _spectrogram.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                {
                    _spectrogram = value;
                    OnPropertyChanged(nameof(SpectrogramImage));
                });
            }
        }
        BitmapImage _spectrogramVetical = new();
        public BitmapImage SpectrogramVerticalImage
        {
            get => _spectrogramVetical;
            private set
            {
                _spectrogramVetical.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                {
                    _spectrogramVetical = value;
                    OnPropertyChanged(nameof(SpectrogramVerticalImage));
                });
            }
        }

        public SpectrogramViewModel()
        {
            Generator = Manager.SpectrogramStream;
            Generator.PropertyChanged += Generator_PropertyChanged;
        }

        private void Generator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SpectrogramGenerator.SpectrogramImage):
                    SpectrogramImage = Generator.SpectrogramImage;
                    break;
                case nameof(SpectrogramGenerator.SpectrogramVerticalImage):
                    SpectrogramVerticalImage = Generator.IsVerticalImageEnabled ? Generator.SpectrogramVerticalImage : new();
                    break;
            }
        }
    }
}
