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


        public SpectrogramViewModel()
        {
            Generator = Manager.SpectrogramStream;
            Generator.ImageAvailable += (sender, image) => SpectrogramImage = image;

        }
    }
}
