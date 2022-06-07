using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AnalyseAudio_PInfo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpectrogramWindow : Window
    {
        public SpectrogramWindow()
        {
            InitializeComponent();
            Closed += SpectrogramWindow_Closed;
            Title = "Spectrogramme";
        }

        static SpectrogramWindow _instance;
        static SpectrogramWindow Instance { get { if (_instance == null) _instance = new SpectrogramWindow(); return _instance; } }
        /// <summary>
		/// Open a new window or focus an existing one
		/// </summary>
        public static void Open()
        {
            _ = Instance;
            Instance.Activate();
        }

        /// <summary>
		/// Close the window if it is open
		/// </summary>
        public new static void Close()
        {
            if (_instance == null) return;
            ((Window)_instance).Close();
            _instance = null;
        }

        private void SpectrogramWindow_Closed(object sender, WindowEventArgs args)
        {
            _instance = null;
        }
    }
}
