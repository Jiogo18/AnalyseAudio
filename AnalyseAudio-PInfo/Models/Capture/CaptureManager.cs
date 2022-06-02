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
        static CaptureManager _instance;
        public static CaptureManager Instance { get { if (_instance == null) _instance = new CaptureManager(); return _instance; } }

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
                if (State == CaptureStatus.Started)
                    _device?.Start(CaptureStream);
                previous?.Stop();
                Logger.WriteLine($"CaptureManager: SelectedDevice is now {value?.DisplayName}");
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

        private CaptureManager()
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
    }
}
