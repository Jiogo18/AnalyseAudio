﻿using AnalyseAudio_PInfo.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class SpectrogramConfigViewModel : ObservableRecipient
    {
        public SpectrogramConfigViewModel()
        {
            SpectrogramGenerator generator = Manager.SpectrogramStream;
            _fftSize = generator.FFTSize;
            _stepSize = generator.StepSize;
            _freqMin = generator.FreqMin;
            _freqMax = generator.FreqMax;
            OnPropertyChanged();
        }

        int _fftSize = 4096;
        public int FFTSize { get => _fftSize; set { if (_fftSize == value) return; _fftSize = value; OnPropertyChanged(nameof(FFTSize)); } }
        public readonly int[] FFTSizes = { 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
        int _stepSize = 200;
        public int StepSize { get => _stepSize; set { if (_stepSize == value) return; _stepSize = value; OnPropertyChanged(nameof(StepSize)); } }
        double _freqMin = 0;
        public double FreqMin
        {
            get => _freqMin;
            set
            {
                if (_freqMin == value) return;
                _freqMin = value;
                if (FreqMax < FreqMin) FreqMax = FreqMin;
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
                if (FreqMax < FreqMin) FreqMin = FreqMax;
                OnPropertyChanged(nameof(FreqMax));
            }
        }

        readonly List<string> PropertiesChanged = new();
        Task TaskWaitUpdate;
        DateTime TimeWaitUpdate;

        public static void OpenSpectrogram()
        {
            SpectrogramWindow.Open();
        }

        bool _autoUpdate = true;
        public bool IsAutoUpdate { get => _autoUpdate; set { if (_autoUpdate == value) return; _autoUpdate = value; OnPropertyChanged(nameof(IsAutoUpdate)); } }

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
            }
        }

        public void FreqMinChanged(object sender, double freq) => FreqMin = freq;
        public void FreqMaxChanged(object sender, double freq) => FreqMax = freq;
    }
}