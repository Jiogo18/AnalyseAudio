
using AnalyseAudio_PInfo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class SpectrogramViewModel : ObservableRecipient
    {
        private BitmapImage _spectrogram;
        public BitmapImage Spectrogram { get => _spectrogram; set { _spectrogram = value; OnPropertyChanged(nameof(Spectrogram)); } }

        public AudioStream CaptureStream;

        public SpectrogramViewModel()
        {
            Spectrogram = new()
            {
                DecodePixelHeight = 80,
                DecodePixelWidth = 80
            };
            CaptureStream = CaptureManager.Initialize().CaptureStream;
        }
    }
}
