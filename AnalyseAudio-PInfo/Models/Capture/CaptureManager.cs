using AnalyseAudio_PInfo.Models.Common;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
	/// The type of devices you can use to capture audio
	/// </summary>
    public enum DeviceType
    {
        Microphone = 0,
        Speaker = 1,
        WaveIn = 2,
    };

    public enum CaptureStatus
    {
        Stopped,
        Started,
    }

    /// <summary>
    /// A Manager to capture and send datas to an AudioStream
    /// </summary>
    public class CaptureManager : NotifyBase
    {
        CaptureStatus _state = CaptureStatus.Stopped;
        /// <summary>
		/// Recording State
		/// </summary>
        public CaptureStatus State { get => _state; private set { if (_state == value) return; _state = value; OnPropertyChanged(nameof(State)); } }

        DeviceCapture _device;
        /// <summary>
		/// Current Device
		/// </summary>
        internal DeviceCapture SelectedDevice
        {
            get { return _device; }
            set
            {
                if (_device == value) return;
                DeviceCapture previous = _device;
                _device = value;
                OnPropertyChanged(nameof(SelectedDevice));
                OnPropertyChanged(nameof(Type));
                if (SelectedDevice != null)
                {
                    SelectedDevice.OnStart += SelectedDevice_OnStart;
                    SelectedDevice.OnStop += SelectedDevice_OnStop;
                    if (State == CaptureStatus.Started)
                        SelectedDevice.Start(CaptureStream, WaveFormat);
                }
                if (previous != null)
                {
                    previous.OnStart -= SelectedDevice_OnStart;
                    previous.OnStop -= SelectedDevice_OnStop;
                    previous.Stop();
                }
                Logger.WriteLine($"CaptureManager: SelectedDevice is now {SelectedDevice?.DisplayName}");
            }
        }

        /// <summary>
        /// Current Type
        /// </summary>
        public DeviceType Type
        {
            get
            {
                if (SelectedDevice is DeviceMicrophone) return DeviceType.Microphone;
                else if (SelectedDevice is DeviceSpeaker) return DeviceType.Speaker;
                return DeviceType.Microphone;
            }
        }

        NAudio.Wave.WaveFormat _waveFormat = new(48000, 8, 1);
        /// <summary>
		/// Current WaveFormat
		/// Bits per samples is recommanded to be 8 for the Spectrogram
		/// Channels must be 1 to have the correct frequency scale
		/// </summary>
        public NAudio.Wave.WaveFormat WaveFormat
        {
            get => _waveFormat;
            set
            {
                if (_waveFormat == value) return;
                _waveFormat = value;
                OnPropertyChanged(nameof(WaveFormat));
                if (State == CaptureStatus.Started)
                    SelectedDevice.WaveFormat = value;
            }
        }

        /// <summary>
        /// The unique AudioStream where Datas are push
        /// </summary>
        public readonly AudioStream CaptureStream = new();

        public CaptureManager() { }

        /// <summary>
        /// Start the recording with the main device
        /// </summary>
        public void Start()
        {
            Logger.WriteLine($"Start recording {SelectedDevice?.DisplayName}");
            State = CaptureStatus.Started;
            SelectedDevice?.Start(CaptureStream, WaveFormat);
        }

        /// <summary>
		/// Stop the recording
		/// </summary>
        public void Stop()
        {
            Logger.WriteLine($"Stop recording {SelectedDevice?.DisplayName}");
            State = CaptureStatus.Stopped;
            SelectedDevice?.Stop();
        }

        /// <summary>
		/// Restart the recording
		/// </summary>
        public void Restart()
        {
            SelectedDevice?.Stop();
            SelectedDevice?.Start(CaptureStream, WaveFormat);
            State = CaptureStatus.Started;
        }

        /// <summary>
		/// SelectedDevice started
		/// </summary>
		/// <param name="sender"></param> SelectedDevice
		/// <param name="e"></param>
        private void SelectedDevice_OnStart(object sender, System.EventArgs e)
        {
            State = CaptureStatus.Started;
        }

        /// <summary>
		/// SelectedDevice endded
		/// </summary>
		/// <param name="sender"></param> SelectedDevice
		/// <param name="e"></param>
        private void SelectedDevice_OnStop(object sender, DeviceCapture.StoppedReason e)
        {
            State = CaptureStatus.Stopped;
        }
    }
}
