
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using Windows.Storage.Streams;

namespace AnalyseAudio_PInfo.ViewModels
{
    internal class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Bitmap bitmap = (Bitmap)value;
            //var imageConverter = new ImageConverter();
            //Byte[] Data = imageConverter.ConvertTo(bitmap, System.Type.GetType("System.Byte[]"));

            MemoryStream stream = new();
            BitmapImage bitmapImage = new();
            IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmapImage.SetSource(randomAccessStream);

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
