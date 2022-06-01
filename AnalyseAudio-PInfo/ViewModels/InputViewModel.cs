
using AnalyseAudio_PInfo.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class InputViewModel : ObservableRecipient
    {
        public InputManager Input { get => InputManager.Instance; }

        public InputViewModel()
        {
            Input?.UpdateDevices();
        }
    }
}
