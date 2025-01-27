using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace AnalyseAudio.ViewModels
{
    /// <summary>
    /// Convert a linear value [0;1000] into a Frequency [1;20000]
    /// 
    /// Due to limitations, if Maximum is close to 1, then the scale won't be logarithmic.
    /// As such, <see cref="InternalScale"/> and set CenterValue.
    /// </summary>
    internal class LogSliderTooltipConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(LogSliderTooltipConverter), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(LogSliderTooltipConverter), new PropertyMetadata(20000.0));
        public static readonly DependencyProperty CenterValueProperty = DependencyProperty.Register("CenterValue", typeof(double), typeof(LogSliderTooltipConverter), new PropertyMetadata(0.0));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public double CenterValue
        {
            get => (double)GetValue(CenterValueProperty);
            set => SetValue(CenterValueProperty, value);
        }

        // Linear values of the slider (internal values)
        public static readonly double MinLinear = 0;
        public static readonly double MaxLinear = 1000;

        /// <summary>
        /// Change the repartition of the values between Min and Max.
        /// Min and Max don't change.
        /// 
        /// e.g. Set 440 in the center of the slider
        /// 
        /// s=\frac{\left(20000-880\right)}{440^{2}}
        /// \left(x\right)=\frac{\ln\left(x\cdot s+1\right)}{\ln\left(20000\cdot s+1\right)}\cdot1000
        /// 
        /// To disable this and keep a scale fully logarithmic, set CenterValue to Minimum.
        /// </summary>
        private double InternalScale => CenterValue == Minimum ? 1 : (Maximum - 2 * CenterValue) / (CenterValue * CenterValue);

        public double ValueToLinear(double freq) =>
            Math.Log((freq - Minimum) * InternalScale + 1)
            / Math.Log((Maximum - Minimum) * InternalScale + 1)
            * (MaxLinear - MinLinear) + MinLinear;
        public double LinearToValue(double value) =>
            (Math.Pow((Maximum - Minimum) * InternalScale + 1,
                (value - MinLinear) / (MaxLinear - MinLinear)) - 1) / InternalScale + Minimum;

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            double val = LinearToValue((double)value);
            if (val == 0) return "0";
            return val >= 100 ? Math.Round(val) : Math.Round(val, 2 - (int)Math.Floor(Math.Log10(val)));
        }

        public object ConvertBack(object freq, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
}
