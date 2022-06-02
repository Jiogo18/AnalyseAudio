using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public class DeviceCaptureManager : NotifyBase
    {
        static DeviceCaptureManager _instance;
        public static DeviceCaptureManager Instance { get { if (_instance == null) _instance = new DeviceCaptureManager(); return _instance; } }

        public bool IsAutoUpdate { get; set; } = true;

        public DeviceCaptureManager()
        {
            UpdateDevices();
        }

        int _type;
        public int SelectedType
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged(nameof(SelectedType));
            }
        }

        public ObservableCollection<DeviceMicrophone> Microphones { get; } = new();
        DeviceMicrophone _microphone;
        public DeviceMicrophone SelectedMicrophone
        {
            get { return _microphone; }
            set
            {
                SelectedType = (int)DeviceType.Microphone;
                if (_microphone == value) return;
                _microphone = value;
                OnPropertyChanged(nameof(SelectedMicrophone));
            }
        }

        public ObservableCollection<DeviceSpeaker> Speakers { get; } = new();
        DeviceSpeaker _speaker;
        public DeviceSpeaker SelectedSpeaker
        {
            get { return _speaker; }
            set
            {
                SelectedType = (int)DeviceType.Speaker;
                if (_speaker == value) return;
                _speaker = value;
                OnPropertyChanged(nameof(SelectedSpeaker));
            }
        }

        public void UpdateDevices()
        {
            bool wasAutoUpdate = IsAutoUpdate;
            IsAutoUpdate = false;
            int previousType = SelectedType;
            List<DeviceMicrophone> Microphones = DeviceMicrophone.ListWASAPI();
            List<DeviceSpeaker> Speakers = DeviceSpeaker.ListWASAPI();
            this.Microphones.Clear();
            Microphones.ForEach(m => this.Microphones.Add(m));
            this.Speakers.Clear();
            Speakers.ForEach(s => this.Speakers.Add(s));

            SelectedMicrophone = SelectDefaultDevice(Microphones, SelectedMicrophone);
            SelectedSpeaker = SelectDefaultDevice(Speakers, SelectedSpeaker);
            SelectedType = previousType;
            if (wasAutoUpdate)
            {
                ApplyChanges();
                IsAutoUpdate = true;
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

        public void RestoreSelection()
        {
            DeviceCapture SelectedDevice = CaptureManager.Instance.SelectedDevice;

            if (SelectedDevice == null)
            {
                if (SelectedMicrophone != null)
                {
                    CaptureManager.Instance.SelectedDevice = SelectedMicrophone;
                    SelectedType = (int)DeviceType.Microphone;
                }
                else if (SelectedSpeaker != null)
                {
                    CaptureManager.Instance.SelectedDevice = SelectedSpeaker;
                    SelectedType = (int)DeviceType.Speaker;
                }
            }
            else
            {
                if (SelectedDevice is DeviceMicrophone) SelectedMicrophone = SelectedDevice as DeviceMicrophone;
                else if (SelectedDevice is DeviceSpeaker) SelectedSpeaker = SelectedDevice as DeviceSpeaker;
            }
        }

        new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (IsAutoUpdate)
                ApplyChanges();
        }

        public void ApplyChanges()
        {
            DeviceWasapi SelectedDevice = SelectedType switch
            {
                (int)DeviceType.Microphone => SelectedMicrophone,
                (int)DeviceType.Speaker => SelectedSpeaker,
                _ => null
            };

            if (SelectedDevice != null)
            {
                CaptureManager.Instance.SelectedDevice = SelectedDevice;
                switch (SelectedDevice.State)
                {
                    case NAudio.CoreAudioApi.DeviceState.Disabled:
                        SendWarning($"The Device {SelectedDevice.DisplayName} is Disabled. You need to enable it in Windows, or choose another device.");
                        break;
                    case NAudio.CoreAudioApi.DeviceState.Unplugged:
                        SendWarning($"The Device {SelectedDevice.DisplayName} is Unplugged. You need to plug it, or choose another device.");
                        break;
                }
            }
        }

        static void SendWarning(string text)
        {
            Logger.Warn(text);
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
