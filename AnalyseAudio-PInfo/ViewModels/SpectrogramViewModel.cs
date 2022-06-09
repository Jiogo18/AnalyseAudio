using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Spectro;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AnalyseAudio_PInfo.ViewModels
{
    /// <summary>
    /// ViewModel for the SpecotrgramElement
    /// </summary>
    public class SpectrogramViewModel : ObservableRecipient
    {
        public readonly SpectrogramGenerator Generator;
        BitmapImage _spectrogram = new();
        /// <summary>
        /// The current spectrogram image.
        /// This image has to be local, so the xaml can access it without interfering with the generator.
        /// </summary>
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
        /// <summary>
        /// The current frequency scale image
        /// This image has to be local, so the xaml can access it without interfering with the generator.
        /// </summary>
        public BitmapImage SpectrogramVerticalImage
        {
            get => _spectrogramVetical;
            private set
            {
                _spectrogramVetical.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
                {
                    _spectrogramVetical = Generator.IsVerticalImageEnabled ? value : new();
                    OnPropertyChanged(nameof(SpectrogramVerticalImage));
                });
            }
        }

        public SpectrogramViewModel()
        {
            Generator = Manager.SpectrogramStream;
            SpectrogramImage = Generator.SpectrogramImage;
            SpectrogramVerticalImage = Generator.SpectrogramVerticalImage;
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
                    SpectrogramVerticalImage = Generator.SpectrogramVerticalImage;
                    break;
            }
        }
    }
}
