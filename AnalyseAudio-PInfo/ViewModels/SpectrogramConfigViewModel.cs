using CommunityToolkit.Mvvm.ComponentModel;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class SpectrogramConfigViewModel : ObservableRecipient
    {
        public SpectrogramConfigViewModel()
        {
        }

        public void OpenSpectrogram()
        {
            SpectrogramWindow.Open();
        }
    }
}