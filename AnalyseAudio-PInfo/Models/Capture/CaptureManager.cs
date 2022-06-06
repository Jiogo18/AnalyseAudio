using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models.Common;

namespace AnalyseAudio_PInfo.Models.Capture
{
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

    public class CaptureManager : NotifyBase
    {
        CaptureStatus _state = CaptureStatus.Stopped;
        public CaptureStatus State { get => _state; private set { if (_state == value) return; _state = value; OnPropertyChanged(nameof(State)); } }

        DeviceCapture _device;
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

        public readonly AudioStream CaptureStream = new();

        public CaptureManager()
        {
        }

        public void Start()
        {
            Logger.WriteLine($"Start recording {SelectedDevice?.DisplayName}");
            State = CaptureStatus.Started;
            SelectedDevice?.Start(CaptureStream, WaveFormat);
        }

        public void Stop()
        {
            Logger.WriteLine($"Stop recording {SelectedDevice?.DisplayName}");
            State = CaptureStatus.Stopped;
            SelectedDevice?.Stop();
        }

        public void Restart()
        {
            SelectedDevice?.Stop();
            SelectedDevice?.Start(CaptureStream, WaveFormat);
            State = CaptureStatus.Started;
        }

        private void SelectedDevice_OnStart(object sender, System.EventArgs e)
        {
            State = CaptureStatus.Started;
        }

        private void SelectedDevice_OnStop(object sender, DeviceCapture.StoppedReason e)
        {
            State = CaptureStatus.Stopped;
        }

    }
}
