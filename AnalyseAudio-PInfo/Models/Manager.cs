using AnalyseAudio_PInfo.Models.Capture;

namespace AnalyseAudio_PInfo.Models
{

    public class Manager
    {
        public static Manager Instance;
        private Manager() { }

        public static void Initialize()
        {
            Instance = new Manager();
            _ = CaptureManager.Instance;
        }
    }
}
