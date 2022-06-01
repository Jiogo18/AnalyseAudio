namespace AnalyseAudio_PInfo.Models
{

    public class Manager
    {
        public static Manager Instance;
        private Manager() { }

        public static void Initialize()
        {
            Instance = new Manager();
            InputManager.Initialize();
        }
    }
}
