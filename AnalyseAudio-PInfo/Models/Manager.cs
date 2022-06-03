using AnalyseAudio_PInfo.Models.Capture;

namespace AnalyseAudio_PInfo.Models
{

    public class Manager
    {
        static Manager instance;
        public static Manager Instance { get { if (instance == null) { instance = new Manager(); instance.Load(); } return instance; } }

        CaptureManager captureManager;
        DeviceCaptureManager deviceCaptureManager;
        SpectrogramGenerator spectrogramStream;

        public static CaptureManager Capture { get => Instance.captureManager; }
        public static DeviceCaptureManager DeviceCapture { get => Instance.deviceCaptureManager; }
        public static SpectrogramGenerator SpectrogramStream { get => Instance.spectrogramStream; }


        private Manager() { }

        public static void Initialize()
        {
            _ = Instance;
        }

        void Load()
        {
            captureManager = new CaptureManager();
            deviceCaptureManager = new DeviceCaptureManager();
            spectrogramStream = new SpectrogramGenerator(Capture.CaptureStream);
        }

        public static void Close()
        {
            if (instance == null) return;
            SpectrogramWindow.Close();
            Capture.Stop();
            instance = null;

        }
    }
}
