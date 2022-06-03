using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models.Common;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public enum DeviceType
    {
        Microphone = 0,
        Speaker = 1
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
                        SelectedDevice.Start(CaptureStream);
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

        public readonly AudioStream CaptureStream = new();

        public CaptureManager()
        {
        }

        public void Start()
        {
            State = CaptureStatus.Started;
            SelectedDevice?.Start(CaptureStream);
        }

        public void Stop()
        {
            State = CaptureStatus.Stopped;
            SelectedDevice?.Stop();
        }

        public void Restart()
        {
            SelectedDevice?.Stop();
            SelectedDevice?.Start(CaptureStream);
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
