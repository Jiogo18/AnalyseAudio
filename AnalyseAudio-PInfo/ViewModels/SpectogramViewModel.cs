using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Capture;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using Spectrogram;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

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
            //SpectrogramImage = new(80, 80);
            CaptureStream = CaptureManager.Instance.CaptureStream;
            if (CaptureStream != null)
                CaptureStream.DataAvailable += CaptureStream_DataAvailable;
            SpectrogramImage = GetBitmapImageTest();
        }

        ~SpectrogramViewModel()
        {
            CaptureStream.DataAvailable -= CaptureStream_DataAvailable;
        }

        private void CreateSampleGenerator(int sampleRate)
        {
            //sampleRate = GetMultipleOfTwoAbove(sampleRate);
            generator = new SpectrogramGenerator(1024, fftSize: 4096, stepSize: 500, maxFreq: 3000, minFreq: 0);
            generator.SetFixedWidth(4096);
            generator.Colormap = Colormap.Lopora;
        }

        private static int GetMultipleOfTwoAbove(int number)
        {
            int result = 2;
            while (result < number) result <<= 1;
            return result;
        }

        //bool IsFirst = true;
        int compteur = 0;
        private void CaptureStream_DataAvailable(object sender, DataReceivedEventArgs e)
        {
            if (generator == null)
                //if (generator == null || generator.SampleRate != e.Length)
                CreateSampleGenerator(e.Length);

            double[] data = new double[e.Length];
            for (int i = 0; i < e.Length; i++)
            {
                data[i] = ((double)e.Data[i]);
            }
            generator.Add(data, true);
            //if (++compteur > 20)
            //{
            //    compteur = 0;
            //    generator.RollReset(0);

            //}
            Bitmap bitmap = generator.GetBitmap(intensity: 100, roll: true);

            // Calc moyenne
            double moyenne = 0, max = data[0], min = data[0];
            for (int i = 0; i < data.Length; i++)
            {
                moyenne += data[i];
                if (data[i] > max) max = data[i];
                if (data[i] < min) min = data[i];
            }
            moyenne /= data.Length;

            Logger.WriteLine($"Moyenne: {moyenne} ({data.Length}), Max: {max}, Min: {min}");




            //Color bottomRight = bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1);
            //Logger.WriteLine($"Pixel bottom right: {bottomRight.R} {bottomRight.G} {bottomRight.B}");

            //_ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => SpectrogramImage = bitmap);
            //_ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => SpectrogramImage = bitmap);
            _ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
            {
                BitmapImage bitmapImage = new();
                // Both works (but better with stream)
                //bitmapImage = await GetBitmapImageAsyncFromBitmap(bitmap);
                bitmapImage = GetBitmapImageFromBitmapWithStream(bitmap);
                //bitmapImage = GetBitmapImageTest();

                SpectrogramImage = bitmapImage;

                //if (IsFirst)
                //{
                //    IsFirst = false;
                //    bitmap.Save("C:/Users/Jerome/Desktop/Image1.bmp");
                //    //Logger.WriteLine($"First image saved");
                //}
            });
        }

        public static Task<BitmapImage> GetBitmapImageAsyncFromBitmap(Bitmap bitmap)
        {
            return GetBitmapImageAsyncFromBytes(BitmapToBytes(bitmap));
        }

        // Works (vérifié avec BitmapToBytes)
        public static Bitmap BytesToBitmap(byte[] byteArray)
        {
            using MemoryStream ms = new(byteArray);
            Bitmap img = (Bitmap)System.Drawing.Image.FromStream(ms);
            return img;
        }

        // Works (vérifié avec BytesToBitmap)
        // https://csharp.hotexamples.com/examples/System.Drawing/ImageConverter/ConvertTo/php-imageconverter-convertto-method-examples.html
        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            ImageConverter imageConverter = new();
            return (byte[])imageConverter.ConvertTo(bitmap, typeof(byte[]));
        }

        // https://stackoverflow.com/a/10892245/12908345
        public static async Task<BitmapImage> GetBitmapImageAsyncFromBytes(byte[] data)
        {
            var bitmapImage = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }

            return bitmapImage;
        }

        // Bitmap => Stream https://stackoverflow.com/a/27389025/12908345
        // Stream => BitmapImage https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapsource.setsource?view=windows-app-sdk-1.0
        public static BitmapImage GetBitmapImageFromBitmapWithStream(Bitmap bitmap)
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

        public static BitmapImage GetBitmapImageTest()
        {
            return new BitmapImage(new Uri("C:/Users/Jerome/Desktop/Image1.bmp", UriKind.Absolute));
        }
    }
}