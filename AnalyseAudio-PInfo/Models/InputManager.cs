using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Models.Common;
using AnalyseAudio_PInfo.Models.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AnalyseAudio_PInfo.Models
{
    public class InputManager : NotifyBase
    {
        public static InputManager Instance { get; private set; }

        public ObservableCollection<DeviceMicrophone> AvailableMicrophones { get; } = new();
        DeviceMicrophone _microphone;
        public DeviceMicrophone SelectedMicrophone
        {
            get { return _microphone; }
            set
            {
                if (_microphone == value) return;
                _microphone = value;
                OnPropertyChanged(nameof(SelectedMicrophone));
                //if (_microphone != null) recognizer?.SetLanguage(_microphone); // TODO: change the input by this device
            }
        }
        public ObservableCollection<DeviceSpeaker> AvailableSpeakers { get; } = new();
        DeviceSpeaker _speaker;
        public DeviceSpeaker SelectedSpeaker
        {
            get { return _speaker; }
            set
            {
                if (_speaker == value) return;
                _speaker = value;
                OnPropertyChanged(nameof(SelectedSpeaker));
                //if (_speaker != null) recognizer?.SetLanguage(_speaker); // TODO: change the input by this device
            }
        }

        private InputManager() { }

        public static void Initialize()
        {
            Instance = new InputManager();
            Instance.UpdateDevices();
        }

        public void UpdateDevices()
        {
            DeviceMicrophone previousSelectedMicrophone = SelectedMicrophone;
            DeviceSpeaker previousSelectedSpeaker = SelectedSpeaker;
            List<DeviceMicrophone> Microphones = DeviceMicrophone.ListWASAPI();
            List<DeviceSpeaker> Speakers = DeviceSpeaker.ListWASAPI();
            AvailableMicrophones.Clear();
            Microphones.ForEach(m => AvailableMicrophones.Add(m));
            AvailableSpeakers.Clear();
            Speakers.ForEach(s => AvailableSpeakers.Add(s));

            // Select the previous device, or the default, or the first
            DeviceMicrophone newSelectedMicrophone = null;
            if (previousSelectedMicrophone != null)
                newSelectedMicrophone = Microphones.Find(x => x.Equals(previousSelectedMicrophone));
            if (newSelectedMicrophone == null)
            {
                newSelectedMicrophone = Microphones.Find(x => x.IsDefault);
                if (newSelectedMicrophone == null && Microphones.Count > 0)
                    newSelectedMicrophone = Microphones[0];
            }
            SelectedMicrophone = newSelectedMicrophone;


            // Select the previous device, or the default, or the first
            DeviceSpeaker newSelectedSpeaker = null;
            if (previousSelectedSpeaker != null)
                newSelectedSpeaker = Speakers.Find(x => x.Equals(previousSelectedSpeaker));
            if (newSelectedSpeaker == null)
            {
                newSelectedSpeaker = Speakers.Find(x => x.IsDefault);
                if (newSelectedSpeaker == null && Speakers.Count > 0)
                    newSelectedSpeaker = Speakers[0];
            }
            SelectedSpeaker = newSelectedSpeaker;

            Logger.WriteLine($"Analyse : devices updated ({AvailableMicrophones.Count} mic, {AvailableSpeakers.Count} speakers)");
        }
    }
}
