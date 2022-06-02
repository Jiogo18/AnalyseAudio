﻿using AnalyseAudio_PInfo.Models;
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
            Resume();
        }

        ~SpectrogramViewModel()
        {
            Pause();
        }

        bool IsCapturing = false;
        public void Resume()
        {
            if (IsCapturing) return;
            CaptureStream.DataAvailable += CaptureStream_DataAvailable;
            IsCapturing = true;
        }

        public void Pause()
        {
            if (!IsCapturing) return;
            CaptureStream.DataAvailable -= CaptureStream_DataAvailable;
            IsCapturing = false;
        }

        private void CreateSampleGenerator(int sampleRate)
        {
            generator = new(sampleRate, fftSize: 4096, stepSize: 200, maxFreq: 2000, fixedWidth: 4096);
        }


        double[] previousInputs = System.Array.Empty<double>();
        double[] previousOutputs = System.Array.Empty<double>();
        private double[] FiltrePasseHaut(double[] inputs)
        {
            double[] outputs = new double[inputs.Length];

            const double a = 0.5;
            double Xn, Yn, Yn_1 = previousOutputs.Length > 0 ? previousOutputs[0] : 0;
            for (int n = 0; n < inputs.Length; n++)
            {
                Xn = inputs[n];
                Yn = a * Yn_1 + Xn;
                outputs[n] = Yn;
                Yn_1 = Yn;
            }
            previousInputs = inputs;
            previousOutputs = outputs;
            return outputs;
        }

        private void CaptureStream_DataAvailable(object sender, DataReceivedEventArgs e)
        {
            if (generator == null || generator.SampleRate != e.SampleRate)
                CreateSampleGenerator(e.SampleRate);

            double[] data = new double[e.Length];
            for (int i = 0; i < e.Length; i++)
                data[i] = e.Data[i];
            generator.Add(data, true);

            Bitmap bitmap = generator.GetBitmap(intensity: 20);
            _ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
            {
                SpectrogramImage = GetBitmapImageFromBitmapWithStream(bitmap);
                bitmap.Dispose();
            });
        }

        // Stream is better (17 ms) than byte[] (400 ms)
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