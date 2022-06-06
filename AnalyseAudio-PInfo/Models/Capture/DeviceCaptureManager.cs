using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public class DeviceCaptureManager : NotifyBase
    {
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

        public ObservableCollection<DeviceWaveIn> WaveInDevices { get; } = new();
        DeviceWaveIn _deviceWaveIn;
        public DeviceWaveIn SelectedWaveIn
        {
            get { return _deviceWaveIn; }
            set
            {
                SelectedType = (int)DeviceType.WaveIn;
                if (_deviceWaveIn == value) return;
                _deviceWaveIn = value;
                OnPropertyChanged(nameof(SelectedWaveIn));
            }
        }

        public void UpdateDevices()
        {
            bool wasAutoUpdate = IsAutoUpdate;
            IsAutoUpdate = false;
            int previousType = SelectedType;
            List<DeviceMicrophone> Microphones = DeviceMicrophone.ListWASAPI();
            List<DeviceSpeaker> Speakers = DeviceSpeaker.ListWASAPI();
            List<DeviceWaveIn> WaveInDevices = DeviceWaveIn.ListDevices();
            this.Microphones.Clear();
            Microphones.ForEach(m => this.Microphones.Add(m));
            this.Speakers.Clear();
            Speakers.ForEach(s => this.Speakers.Add(s));
            this.WaveInDevices.Clear();
            WaveInDevices.ForEach(w => this.WaveInDevices.Add(w));

            SelectedMicrophone = SelectDefaultDevice(Microphones, SelectedMicrophone);
            SelectedSpeaker = SelectDefaultDevice(Speakers, SelectedSpeaker);
            SelectedWaveIn = SelectDefaultDevice(WaveInDevices, SelectedWaveIn);
            SelectedType = previousType;
            if (wasAutoUpdate)
            {
                ApplyChanges();
                IsAutoUpdate = true;
            }
        }

        // Select the previous device, or the default, or the first
        static T SelectDefaultDevice<T>(List<T> Devices, T previous) where T : DeviceCapture
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
            DeviceCapture SelectedDevice = Manager.Capture.SelectedDevice;

            if (SelectedDevice == null)
            {
                if (SelectedMicrophone != null)
                {
                    Manager.Capture.SelectedDevice = SelectedMicrophone;
                    SelectedType = (int)DeviceType.Microphone;
                }
                else if (SelectedSpeaker != null)
                {
                    Manager.Capture.SelectedDevice = SelectedSpeaker;
                    SelectedType = (int)DeviceType.Speaker;
                }
                else if (SelectedWaveIn != null)
                {
                    Manager.Capture.SelectedDevice = SelectedWaveIn;
                    SelectedType = (int)DeviceType.WaveIn;
                }
            }
            else
            {
                if (SelectedDevice is DeviceMicrophone) SelectedMicrophone = SelectedDevice as DeviceMicrophone;
                else if (SelectedDevice is DeviceSpeaker) SelectedSpeaker = SelectedDevice as DeviceSpeaker;
                else if (SelectedDevice is DeviceWaveIn) SelectedWaveIn = SelectedDevice as DeviceWaveIn;
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
            DeviceCapture SelectedDevice = SelectedType switch
            {
                (int)DeviceType.Microphone => SelectedMicrophone,
                (int)DeviceType.Speaker => SelectedSpeaker,
                (int)DeviceType.WaveIn => SelectedWaveIn,
                _ => null
            };

            if (SelectedDevice != null)
            {
                Manager.Capture.SelectedDevice = SelectedDevice;
                if (SelectedDevice is DeviceWasapi)
                {
                    switch ((SelectedDevice as DeviceWasapi).State)
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
        }

        static void SendWarning(string text)
        {
            Logger.Warn(text);
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
