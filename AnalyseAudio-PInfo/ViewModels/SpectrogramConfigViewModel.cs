using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Spectro;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyseAudio_PInfo.ViewModels
{
    /// <summary>
    /// ViewModel for the SpectrogramConfigPage.
    /// </summary>
    public class SpectrogramConfigViewModel : ObservableRecipient
    {
        public SpectrogramConfigViewModel()
        {
            SpectrogramGenerator generator = Manager.SpectrogramStream;
            _fftSize = generator.FFTSize;
            _stepSize = generator.StepSize;
            _fixedSize = generator.FixedWidth;
            _freqMin = generator.FreqMin;
            _freqMax = generator.FreqMax;
            _intensity = generator.Intensity;
            _verticalImage = generator.IsVerticalImageEnabled;
            _viewdB = generator.IsdB;
            _roll = generator.IsRoll;
            OnPropertyChanged();
        }

        int _fftSize = 4096;
        public int FFTSize { get => _fftSize; set { if (_fftSize == value) return; _fftSize = value; OnPropertyChanged(nameof(FFTSize)); } }
        public readonly int[] FFTSizes = { 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
        int _stepSize = 200;
        public int StepSize { get => _stepSize; set { if (_stepSize == value) return; _stepSize = value; OnPropertyChanged(nameof(StepSize)); } }
        int _fixedSize = 4096;
        public int FixedSize { get => _fixedSize; set { if (_fixedSize == value) return; _fixedSize = value; OnPropertyChanged(nameof(FixedSize)); } }
        double _freqMin = 0;
        public double FreqMin
        {
            get => _freqMin;
            set
            {
                if (_freqMin == value) return;
                _freqMin = value;
                if (FreqMax <= FreqMin) FreqMax = FreqMin + 1;
                OnPropertyChanged(nameof(FreqMin));
            }
        }
        double _freqMax = 3000;
        public double FreqMax
        {
            get => _freqMax;
            set
            {
                if (_freqMax == value) return;
                _freqMax = value;
                if (FreqMax <= FreqMin) FreqMin = FreqMax - 1;
                OnPropertyChanged(nameof(FreqMax));
            }
        }

        double _intensity = 1;
        public double Intensity { get => _intensity; set { if (_intensity == value) return; _intensity = value; OnPropertyChanged(nameof(Intensity)); } }
        bool _verticalImage = false;
        public bool VerticalImage { get => _verticalImage; set { if (_verticalImage == value) return; _verticalImage = value; OnPropertyChanged(nameof(VerticalImage)); } }
        bool _viewdB = false;
        public bool DB { get => _viewdB; set { if (_viewdB == value) return; _viewdB = value; OnPropertyChanged(nameof(DB)); } }
        bool _roll = false;
        public bool Roll { get => _roll; set { if (_roll == value) return; _roll = value; OnPropertyChanged(nameof(Roll)); } }

        readonly List<string> PropertiesChanged = new();
        Task TaskWaitUpdate;
        DateTime TimeWaitUpdate;

        public static void OpenSpectrogram()
        {
            SpectrogramWindow.Open();
        }

        bool _autoUpdate = true;
        public bool IsAutoUpdate { get => _autoUpdate; set { if (_autoUpdate == value) return; _autoUpdate = value; OnPropertyChanged(nameof(IsAutoUpdate)); } }

        /// <summary>
        /// Update the spectrogram and spectrogram generator settings.
        /// </summary>
        public void Update()
        {
            PropertiesChanged.ForEach(propertyName => UpdateProperty(propertyName));
            PropertiesChanged.Clear();
        }

        new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(FFTSize):
                case nameof(StepSize):
                case nameof(FreqMin):
                case nameof(FreqMax):
                    PropertiesChanged.Add("Generator");
                    break;
                case nameof(IsAutoUpdate):
                    Update();
                    break;
                default:
                    PropertiesChanged.Add(propertyName);
                    break;
            }

            if (IsAutoUpdate)
            {
                bool WaitBeforeUpdating = propertyName switch
                {
                    "FFTSize" => false,
                    "FixedSize" => false,
                    "IsAutoUpdate" => false,
                    "VerticalImage" => false,
                    "DB" => false,
                    "Roll" => false,
                    _ => true,
                };

                if (!WaitBeforeUpdating)
                {
                    Update();
                    TaskWaitUpdate?.Dispose();
                    TaskWaitUpdate = null;
                    return;
                }

                // Update later
                TimeWaitUpdate = DateTime.Now.AddMilliseconds(500);
                if (TaskWaitUpdate == null)
                {
                    TaskWaitUpdate = Task.Run(async () =>
                    {
                        DateTime now = DateTime.Now;
                        while (now < TimeWaitUpdate)
                        {
                            await Task.Delay(TimeWaitUpdate - now);
                            now = DateTime.Now;
                        }
                        TaskWaitUpdate = null;
                        Update();
                    });
                }
            }
        }

        /// <summary>
        /// Upadte a unique property to the SpectrogramGenerator.
        /// </summary>
        /// <param name="propertyName"></param>
        void UpdateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Generator":
                    Manager.SpectrogramStream.CreateGenerator(new SpectrogramConfig(Manager.SpectrogramStream.Generator)
                    {
                        FreqMin = FreqMin,
                        FreqMax = FreqMax,
                        FFTSize = FFTSize,
                        StepSize = StepSize
                    });
                    break;
                case nameof(Intensity):
                    Manager.SpectrogramStream.Intensity = Intensity;
                    break;
                case nameof(FixedSize):
                    Manager.SpectrogramStream.FixedWidth = FixedSize;
                    break;
                case nameof(VerticalImage):
                    Manager.SpectrogramStream.IsVerticalImageEnabled = VerticalImage;
                    break;
                case nameof(DB):
                    Manager.SpectrogramStream.IsdB = DB;
                    break;
                case nameof(Roll):
                    Manager.SpectrogramStream.IsRoll = Roll;
                    break;
            }
        }
    }
}