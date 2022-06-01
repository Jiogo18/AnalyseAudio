using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Capture;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class CaptureViewModel : ObservableRecipient
    {
        public readonly CaptureManager Capture;
        public static bool IsAutoUpdate { get; set; } = true;
        readonly List<string> ModifiedProperties = new();

        int _selectedType = 0;
        public int SelectedType
        {
            get => _selectedType; set
            {
                if (_selectedType == value) return;
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
            }
        }

        public ObservableCollection<DeviceMicrophone> AvailableMicrophones { get; } = new();
        DeviceMicrophone _microphone;
        public DeviceMicrophone SelectedMicrophone
        {
            get { return _microphone; }
            set
            {
                if (_microphone == value) return;
                _microphone = value;
                OnPropertyChanged(nameof(SelectedMicrophone));
                //if (_microphone != null) recognizer?.SetLanguage(_microphone); // TODO: change the Capture by this device
            }
        }
        public ObservableCollection<DeviceSpeaker> AvailableSpeakers { get; } = new();
        DeviceSpeaker _speaker;
        public DeviceSpeaker SelectedSpeaker
        {
            get { return _speaker; }
            set
            {
                if (_speaker == value) return;
                _speaker = value;
                OnPropertyChanged(nameof(SelectedSpeaker));
            }
        }

        public CaptureViewModel()
        {
            Capture = CaptureManager.Initialize();
            UpdateDevices();
        }

        public void UpdateDevices()
        {
            bool WasAutoUpdate = IsAutoUpdate;
            IsAutoUpdate = false;

            SelectedType = (int)Capture.Type;

            List<DeviceMicrophone> Microphones = DeviceMicrophone.ListWASAPI();
            List<DeviceSpeaker> Speakers = DeviceSpeaker.ListWASAPI();
            AvailableMicrophones.Clear();
            Microphones.ForEach(m => AvailableMicrophones.Add(m));
            AvailableSpeakers.Clear();
            Speakers.ForEach(s => AvailableSpeakers.Add(s));

            SelectedMicrophone = SelectDefaultDevice(Microphones, Capture.Type == DeviceType.Microphone ? (DeviceMicrophone)Capture.SelectedDevice : null);
            SelectedSpeaker = SelectDefaultDevice(Speakers, Capture.Type == DeviceType.Speaker ? (DeviceSpeaker)Capture.SelectedDevice : null);

            if (WasAutoUpdate)
            {
                IsAutoUpdate = true;
                Update();
            }

        }

        // Select the previous device, or the default, or the first
        static T SelectDefaultDevice<T>(List<T> Devices, T previous) where T : DeviceWasapi
        {
            T newSelected = null;
            if (previous != null)
                newSelected = Devices.Find(x => x.Equals(previous));
            if (newSelected == null)
            {
                newSelected = Devices.Find(x => x.IsDefault);
                if (newSelected == null && Devices.Count > 0)
                    newSelected = Devices[0];
            }
            return newSelected;
        }

        new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedType) || propertyName == nameof(SelectedMicrophone) || propertyName == nameof(SelectedSpeaker))
                propertyName = "Device";
            if (IsAutoUpdate)
            {
                UpdateProperty(propertyName);
            }
            else
            {
                ModifiedProperties.Add(propertyName);
            }
        }

        public void StartStop()
        {
            if (Capture.State == CaptureStatus.Stopped)
                Capture.Start();
            else
                Capture.Stop();
        }

        // Update the Capture manager
        public void Update()
        {
            if (Capture == null) return;
            ModifiedProperties.ForEach(UpdateProperty);
            ModifiedProperties.Clear();
        }

        void UpdateProperty(string propertyName)
        {
            if (Capture == null) return;
            switch (propertyName)
            {
                case "Device":
                    switch (SelectedType)
                    {
                        case (int)DeviceType.Microphone:
                            Capture.SelectedDevice = SelectedMicrophone;
                            break;
                        case (int)DeviceType.Speaker:
                            Capture.SelectedDevice = SelectedSpeaker;
                            break;
                    }
                    break;
            }
        }
    }
}
