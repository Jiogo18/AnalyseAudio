using AnalyseAudio_PInfo.Models.Capture;
using Microsoft.UI.Xaml.Data;
using System;

namespace AnalyseAudio_PInfo.ViewModels
{
    /// <summary>
	/// Converter from CaptureStatus to Button Text
	/// </summary>
    internal class StateButtonStartStopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return (CaptureStatus)value switch
            {
                CaptureStatus.Started => "Stop",
                CaptureStatus.Stopped => "Start",
                _ => throw new ArgumentException($"CaptureStatus unknown {value}"),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
