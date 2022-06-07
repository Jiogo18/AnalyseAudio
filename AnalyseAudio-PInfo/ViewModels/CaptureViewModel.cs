using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Capture;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.ViewModels
{
    /// <summary>
	/// ViewModel of CapturePage
	/// </summary>
    public class CaptureViewModel : ObservableRecipient
    {
        public readonly CaptureManager Capture;
        public readonly DeviceCaptureManager Devices;
        readonly List<string> PropertiesChanged = new();
        public bool IsAutoUpdate
        {
            get => Devices.IsAutoUpdate;
            set { Devices.IsAutoUpdate = value; if (value) Update(); OnPropertyChanged(nameof(IsAutoUpdate)); }
        }

        int _sampleRate;
        public int SampleRate { get => _sampleRate; set { if (_sampleRate == value) return; _sampleRate = value; OnPropertyChanged(nameof(SampleRate)); } }
        int _channels;
        public int Channels { get => _channels; set { if (_channels == value) return; _channels = value; OnPropertyChanged(nameof(Channels)); } }
        int _bitsPerSample;
        public int BitsPerSample { get => _bitsPerSample; set { if (_bitsPerSample == value) return; _bitsPerSample = value; OnPropertyChanged(nameof(BitsPerSample)); } }

        public CaptureViewModel()
        {
            Capture = Manager.Capture;
            _sampleRate = Capture.WaveFormat.SampleRate;
            _channels = Capture.WaveFormat.Channels;
            _bitsPerSample = Capture.WaveFormat.BitsPerSample;
            Devices = Manager.DeviceCapture;
            Devices.RestoreSelection();
        }

        public void StartStop()
        {
            if (Capture.State == CaptureStatus.Stopped)
                Capture.Start();
            else
                Capture.Stop();
        }

        private new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(SampleRate):
                case nameof(Channels):
                case nameof(BitsPerSample):
                // If any of these properties change, the wave format must be updated
                    PropertiesChanged.Add("WaveFormat");
                    break;
                default:
                    PropertiesChanged.Add(propertyName);
                    break;
            }

            if (IsAutoUpdate)
            {
                Update();
            }
        }

        /// <summary>
		/// Update the Capture manager
		/// </summary>
        public void Update()
        {
            if (Capture == null) return;
            PropertiesChanged.ForEach(UpdateProperty);
            PropertiesChanged.Clear();
            Devices.ApplyChanges();
        }

        /// <summary>
		/// Update the Capture manager with a single property (or a "category" of properties)
		/// </summary>
		/// <param name="propertyName"></param>
        void UpdateProperty(string propertyName)
        {
            if (Capture == null) return;
            switch (propertyName)
            {
                case "WaveFormat":
                    Capture.WaveFormat = new WaveFormat(SampleRate, BitsPerSample, Channels);
                    break;
            }
        }
    }
}
