using AnalyseAudio_PInfo.Models.Common;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AnalyseAudio_PInfo.Models
{
    public class SpectrogramGenerator : NotifyBase
    {
        bool IsCapturing = false;
        readonly List<object> ViewsOpen = new();
        readonly AudioStream CaptureStream;
        Spectrogram.SpectrogramGenerator generator;
        internal Spectrogram.SpectrogramGenerator Generator { get => generator; }
        public readonly BitmapImage SpectrogramImage = new();
        public readonly BitmapImage SpectrogramVerticalImage = new();

        public SpectrogramGenerator(AudioStream stream)
        {
            CaptureStream = stream;
            CreateGenerator(new SpectrogramConfig());
        }

        void Resume()
        {
            if (IsCapturing) return;
            CaptureStream.DataAvailable += CaptureStream_DataAvailable;
            IsCapturing = true;
        }

        void Pause()
        {
            if (!IsCapturing) return;
            CaptureStream.DataAvailable -= CaptureStream_DataAvailable;
            IsCapturing = false;
        }

        bool HasViewOutput()
        {
            return ViewsOpen.Count > 0;
        }

        public void ViewWantsSpectrogramImage(object view)
        {
            if (!ViewsOpen.Contains(view))
                ViewsOpen.Add(view);
            if (!IsCapturing)
                Resume();
        }

        public void ViewClosed(object view)
        {
            ViewsOpen.Remove(view);
            if (!HasViewOutput())
                Pause();
        }

        public void CreateGenerator(SpectrogramConfig config)
        {
            generator = config.CreateGenerator();
            generator.SetFixedWidth(FixedWidth);
            UpdateVerticalImage();
            OnPropertyChanged();
        }

        public SpectrogramConfig Config => new(generator);
        public int SampleRate => generator.SampleRate;
        public int FFTSize => generator.FftSize;
        public int StepSize => generator.StepSize;
        public double FreqMin => generator.FreqMin;
        public double FreqMax => generator.FreqMax;
        public double Intensity { get; set; } = 20;
        public int FixedWidth { get; set; } = 4096;
        bool _verticalImageEnabled = false;
        public bool IsVerticalImageEnabled
        {
            get => _verticalImageEnabled;
            set { if (_verticalImageEnabled == value) return; _verticalImageEnabled = value; OnPropertyChanged(nameof(IsVerticalImageEnabled)); UpdateVerticalImage(); }
        }


        //double[] previousInputs = System.Array.Empty<double>();
        //double[] previousOutputs = System.Array.Empty<double>();
        //private double[] FiltrePasseHaut(double[] inputs)
        //{
        //    double[] outputs = new double[inputs.Length];

        //    const double a = 0.5;
        //    double Xn, Yn, Yn_1 = previousOutputs.Length > 0 ? previousOutputs[0] : 0;
        //    for (int n = 0; n < inputs.Length; n++)
        //    {
        //        Xn = inputs[n];
        //        Yn = a * Yn_1 + Xn;
        //        outputs[n] = Yn;
        //        Yn_1 = Yn;
        //    }
        //    previousInputs = inputs;
        //    previousOutputs = outputs;
        //    return outputs;
        //}

        private void CaptureStream_DataAvailable(object sender, DataReceivedEventArgs e)
        {
            if (generator.SampleRate != e.SampleRate)
            {
                CreateGenerator(new SpectrogramConfig(generator) { SampleRate = e.SampleRate });
            }

            double[] data = new double[e.Length];
            for (int i = 0; i < e.Length; i++)
                data[i] = e.Data[i];
            generator.Add(data, true);

            Bitmap bitmap = generator.GetBitmap(Intensity);
            SpectrogramImage.DispatcherQueue?.TryEnqueue(DispatcherQueuePriority.Low, () =>
            {
                SetBitmapImageWithBitmapAndStream(bitmap, SpectrogramImage);
                OnPropertyChanged(nameof(SpectrogramImage));
                bitmap.Dispose();
            });
        }

        // Stream is better (17 ms) than byte[] (400 ms)
        // Bitmap => Stream https://stackoverflow.com/a/27389025/12908345
        // Stream => BitmapImage https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapsource.setsource?view=windows-app-sdk-1.0
        private static BitmapImage SetBitmapImageWithBitmapAndStream(Bitmap bitmap, BitmapImage bitmapImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin); //go back to start
                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
            }
            return bitmapImage;
        }

        private void UpdateVerticalImage()
        {
            Bitmap verticalBitmap = generator.GetVerticalScale(80, 0, 1);
            SpectrogramVerticalImage.DispatcherQueue.TryEnqueue(() =>
            {
                SetBitmapImageWithBitmapAndStream(verticalBitmap, SpectrogramVerticalImage);
                OnPropertyChanged(nameof(SpectrogramVerticalImage));
            });
        }
    }
}
