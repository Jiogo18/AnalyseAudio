using AnalyseAudio.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnalyseAudio.Views
{
    /// <summary>
    /// A Slider that can be used (originally for the frequency of the spectrogram).
    /// The slider is logarithmic-like so moving of X pixels will always multiply the frequency with a factor Y.
    /// The slider is centered at 440 Hz (CenterValue) <see cref="LogSliderTooltipConverter.InternalScale"/>.
    /// </summary>
    public sealed partial class LogSlider : Grid, INotifyPropertyChanged
    {
        public static readonly DependencyProperty StepFrequencyProperty = DependencyProperty.Register("StepFrequency", typeof(double), typeof(LogSlider), new PropertyMetadata(0.1));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(LogSlider), new PropertyMetadata(0.0));
        private LogSliderTooltipConverter scaleConverter = new();
        public double MinLinear => LogSliderTooltipConverter.MinLinear;
        public double MaxLinear => LogSliderTooltipConverter.MaxLinear;
        public double Minimum
        {
            get => scaleConverter.Minimum;
            set => scaleConverter.Minimum = value;
        }
        public double Maximum
        {
            get => scaleConverter.Maximum;
            set => scaleConverter.Maximum = value;
        }
        public double CenterValue
        {
            get => scaleConverter.CenterValue;
            set => scaleConverter.CenterValue = value;
        }

        public double StepFrequency
        {
            get => (double)GetValue(StepFrequencyProperty);
            set => SetValue(StepFrequencyProperty, value);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                if (Value == value || EcartRelatif(Value, value) < 0.001) return;
                SetValue(ValueProperty, value);
                OnPropertyChanged(nameof(ValueLinear));
            }
        }

        private double ValueLinear
        {
            get => scaleConverter.ValueToLinear(Value);
            set { Value = scaleConverter.LinearToValue(value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public LogSlider()
        {
            InitializeComponent();
        }

        static double EcartRelatif(double a, double b) => Math.Abs((a - b) / (a != 0 ? a : 1));
    }
}
