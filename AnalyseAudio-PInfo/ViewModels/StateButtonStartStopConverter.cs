﻿using AnalyseAudio_PInfo.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace AnalyseAudio_PInfo.ViewModels
{
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
