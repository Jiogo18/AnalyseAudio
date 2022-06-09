using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnalyseAudio_PInfo.Views
{
    /// <summary>
    /// A Slider that can be used to change the frequency of the spectrogram.
    /// The slider is logarithmic-like so moving of X pixels will always multiply the frequency with a factor Y.
    /// The slider is centered at 440 Hz.
    /// </summary>
    public sealed partial class FrequencySlider : Grid, INotifyPropertyChanged
    {
        double _minimum = 1;
        public double Minimum { get => _minimum; set { if (_minimum == value) return; _minimum = value; OnPropertyChanged(); OnPropertyChanged(nameof(MinLinear)); } }

        double _maximum = 20000;
        public double Maximum { get => _maximum; set { if (_maximum == value) return; _maximum = value; OnPropertyChanged(); OnPropertyChanged(nameof(MaxLinear)); } }


        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register("Frequency", typeof(double), typeof(Grid), new PropertyMetadata(0.0));
        public double Frequency
        {
            get { return (double)GetValue(FrequencyProperty); }
            set
            {
                if (Frequency == value || EcartRelatif(Frequency, value) < 0.001) return;
                SetValue(FrequencyProperty, value);
                OnPropertyChanged(nameof(ValueLinear));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }


        private double MinLinear => FrequencyToLinear(Minimum); // 0
        private double MaxLinear => FrequencyToLinear(Maximum); // 4000
        private double ValueLinear => FrequencyToLinear(Frequency);


        public FrequencySlider()
        {
            InitializeComponent();
        }


        static double FrequencyToLinear(double freq) => Math.Log((freq + 9) / 10) / Math.Log(2000) * 4000;
        static double LinearToFrequency(double value) => Math.Pow(2000, value / 4000) * 10 - 9;
        static double EcartRelatif(double a, double b) => Math.Abs((a - b) / (a != 0 ? a : 1));

        private void Slider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Frequency = LinearToFrequency(e.NewValue);
        }
    }
}
