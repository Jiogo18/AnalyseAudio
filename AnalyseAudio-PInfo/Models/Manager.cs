using AnalyseAudio_PInfo.Models.Capture;
using AnalyseAudio_PInfo.Models.Spectro;

namespace AnalyseAudio_PInfo.Models
{
    /// <summary>
    /// The main object of the application.
    /// This contains the Capture and the Analyze managers
    /// </summary>
    public class Manager
    {
        static Manager instance;
        /// <summary>
        /// Get the current Manager or create a new one
        /// </summary>
        public static Manager Instance { get { if (instance == null) { instance = new Manager(); instance.Load(); } return instance; } }

        CaptureManager captureManager;
        DeviceCaptureManager deviceCaptureManager;
        SpectrogramGenerator spectrogramStream;

        public static CaptureManager Capture { get => Instance.captureManager; }
        public static DeviceCaptureManager DeviceCapture { get => Instance.deviceCaptureManager; }
        public static SpectrogramGenerator SpectrogramStream { get => Instance.spectrogramStream; }


        private Manager() { }

        /// <summary>
        /// Create the manager
        /// </summary>
        public static void Initialize()
        {
            _ = Instance;
        }

        /// <summary>
        /// Create the manager, once Instance is set
        /// </summary>
        private void Load()
        {
            captureManager = new CaptureManager();
            deviceCaptureManager = new DeviceCaptureManager();
            spectrogramStream = new SpectrogramGenerator(Capture.CaptureStream);
        }

        /// <summary>
        /// Stop the application
        /// </summary>
        public static void Close()
        {
            if (instance == null) return;
            SpectrogramWindow.Close();
            Capture.Stop();
            instance = null;

        }
    }
}
