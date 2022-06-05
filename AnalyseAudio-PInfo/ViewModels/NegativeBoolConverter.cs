﻿using Microsoft.UI.Xaml.Data;
using System;

namespace AnalyseAudio_PInfo.ViewModels
{
    internal class NegativeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
