using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Capture;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Spectrogram;
using System.Drawing;
using System.IO;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class SpectrogramViewModel : ObservableRecipient
    {
        private BitmapImage _spectrogram = new();
        public BitmapImage SpectrogramImage
        {
            get => _spectrogram;
            private set
            {
                _spectrogram = value;
                OnPropertyChanged(nameof(SpectrogramImage));
            }
        }

        public DispatcherQueue DispatcherQueue { get; set; }

        public AudioStream CaptureStream;
        SpectrogramGenerator generator;

        public SpectrogramViewModel()
        {
            CaptureStream = CaptureManager.Instance.CaptureStream;
            CreateSampleGenerator(1024); // TODO: Config for a power of 2
            Resume();
        }

        ~SpectrogramViewModel()
        {
            Pause();
        }

        public void Resume()
        {
            CaptureStream.DataAvailable += CaptureStream_DataAvailable;
        }

        public void Pause()
        {
            CaptureStream.DataAvailable -= CaptureStream_DataAvailable;
        }

        private void CreateSampleGenerator(int sampleRate)
        {
            generator = new(sampleRate, fftSize: 4096, stepSize: 500, maxFreq: 3000, fixedWidth: 4096);
        }

        private void CaptureStream_DataAvailable(object sender, DataReceivedEventArgs e)
        {
            double[] data = new double[e.Length];
            for (int i = 0; i < e.Length; i++)
            {
                data[i] = e.Data[i] * 20;
            }
            generator.Add(data, true);

            Bitmap bitmap = generator.GetBitmap();
            // Stream is better (17 ms) than byte[] (400 ms)
            //_ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => SpectrogramImage = bitmap);
            _ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                SpectrogramImage = GetBitmapImageFromBitmapWithStream(bitmap);
                bitmap.Dispose();
            });
        }

        // Bitmap => Stream https://stackoverflow.com/a/27389025/12908345
        // Stream => BitmapImage https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapsource.setsource?view=windows-app-sdk-1.0
        private static BitmapImage GetBitmapImageFromBitmapWithStream(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new();
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin); //go back to start
                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
            }
            return bitmapImage;
        }
    }
}