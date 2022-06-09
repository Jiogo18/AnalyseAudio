using AnalyseAudio_PInfo.Models.Common;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AnalyseAudio_PInfo.Models.Spectro
{
    /// <summary>
    /// An interface for Spectrogram.SpectrogramGenerator
    /// </summary>
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

        /// <summary>
        /// Register a new view to receive spectrogram data.
        /// If there is no views registered, the spectrogram will not be generated.
        /// </summary>
        /// <param name="view"></param>
        public void ViewWantsSpectrogramImage(object view)
        {
            if (!ViewsOpen.Contains(view))
                ViewsOpen.Add(view);
            if (!IsCapturing)
                Resume();
        }

        /// <summary>
        /// Unregister a view from receiving spectrogram data.
        /// If there are no views registered, the spectrogram will not be generated.
        /// </summary>
        /// <param name="view"></param>
        public void ViewClosed(object view)
        {
            ViewsOpen.Remove(view);
            if (!HasViewOutput())
                Pause();
        }

        /// <summary>
        /// (Re)create the generator with a new configuration.
        /// </summary>
        /// <param name="config"></param>
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
        public double Intensity { get; set; } = 200;
        int _fixedWidth = 5000;
        public int FixedWidth
        {
            get => _fixedWidth;
            set { if (_fixedWidth == value) return; _fixedWidth = value; OnPropertyChanged(nameof(FixedWidth)); Generator?.SetFixedWidth(value); }
        }
        bool _verticalImageEnabled = false;
        public bool IsVerticalImageEnabled
        {
            get => _verticalImageEnabled;
            set { if (_verticalImageEnabled == value) return; _verticalImageEnabled = value; OnPropertyChanged(nameof(IsVerticalImageEnabled)); UpdateVerticalImage(); }
        }
        public bool IsdB { get; set; } = false;
        public bool IsRoll { get; set; } = false;


        /// <summary>
        /// When AudioStream has datas, update the spectrogram image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureStream_DataAvailable(object sender, DataReceivedEventArgs e)
        {
            if (generator.SampleRate != e.SampleRate)
            {
                CreateGenerator(new SpectrogramConfig(generator) { SampleRate = e.SampleRate });
            }

            double[] data = new double[e.Length];
            for (int i = 0; i < e.Length; i++)
                data[i] = e.Data[i];
            lock (generator)
                generator.Add(data, true);

            try
            {
                Bitmap bitmap = generator.GetBitmap(Intensity, dB: IsdB, roll: IsRoll);
                SpectrogramImage.DispatcherQueue?.TryEnqueue(DispatcherQueuePriority.Low, () =>
                {
                    SetBitmapImageWithBitmapAndStream(bitmap, SpectrogramImage);
                    OnPropertyChanged(nameof(SpectrogramImage));
                    bitmap.Dispose();
                });
            }
            catch (Exception) { } // Recreating the generator
        }

        /// <summary>
        /// Save a Bitmap into a BitmapImage.
        /// This takes ~17 ms
        /// </summary>
        // Bitmap => Stream https://stackoverflow.com/a/27389025/12908345
        // Stream => BitmapImage https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapsource.setsource?view=windows-app-sdk-1.0
        /// <param name="bitmap"></param> The bitmap source
        /// <param name="bitmapImage"></param> The bitmap target
        /// <returns></returns> The bitmap target
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

        /// <summary>
        /// Update the vertical image with frequency scale
        /// </summary>
        private void UpdateVerticalImage()
        {
            Bitmap verticalBitmap = generator.GetVerticalScale(80);
            SpectrogramVerticalImage.DispatcherQueue.TryEnqueue(() =>
            {
                SetBitmapImageWithBitmapAndStream(verticalBitmap, SpectrogramVerticalImage);
                OnPropertyChanged(nameof(SpectrogramVerticalImage));
            });
        }
    }
}
