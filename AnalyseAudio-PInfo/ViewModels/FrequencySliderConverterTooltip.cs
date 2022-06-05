using Microsoft.UI.Xaml.Data;
using System;

namespace AnalyseAudio_PInfo.ViewModels
{
    /// <summary>
    /// Convert a linear value [0;1000] into a Frequency [1;20000]
    /// </summary>
    internal class FrequencySliderTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            double freq = Math.Pow(2000, (double)value / 4000) * 10 - 9;
            return freq >= 100 ? Math.Round(freq) : Math.Round(freq, 2 - (int)Math.Floor(Math.Log10(freq)));
        }

        public object ConvertBack(object freq, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
