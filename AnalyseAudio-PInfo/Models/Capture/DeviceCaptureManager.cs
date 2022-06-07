using AnalyseAudio_PInfo.Models.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
	/// The manager of your Devices.
	/// Stores Microphones, Speakers and WavesIn.
	/// This is a bridge between CaptureViewModel and CaptureManager.
	/// </summary>
    public class DeviceCaptureManager : NotifyBase
    {
        /// <summary>
		/// Automaticly update the changes made on the CapturePage
		/// </summary>
        public bool IsAutoUpdate { get; set; } = true;

        public DeviceCaptureManager()
        {
            UpdateDevices();
        }

        int _type;
        /// <summary>
        /// 3 types availables: Microphone, Speakers and WavesIn (0, 1, 2)
        /// </summary>
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

        /// <summary>
		/// Refresh the Microphones, Speakers and WaveInDevices
		/// The currently selected of each category may not change
		/// </summary>
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

        /// <summary>
        // Select the previous device, or the default, or the first
        /// </summary>
        /// <typeparam name="T"></typeparam> The type of devices
        /// <param name="Devices"></param> Available devices
        /// <param name="previous"></param> The device selected before
        /// <returns></returns>/
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

        /// <summary>
        /// Refresh the selection with the current device used by CaptureManager
        /// </summary>
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

        /// <summary>
		/// Event ProperyChanged and Apply changes
		/// </summary>
		/// <param name="propertyName"></param>
        new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (IsAutoUpdate)
                ApplyChanges();
        }

        /// <summary>
		/// Update CaptureManager with this configuration (Save)
		/// </summary>
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

        /// <summary>
		/// Alert the user and register it in the logs
		/// </summary>
		/// <param name="text"></param> The message
        static void SendWarning(string text)
        {
            Logger.Warn(text);
            MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
