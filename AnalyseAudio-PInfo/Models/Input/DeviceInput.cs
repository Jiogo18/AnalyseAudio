using AnalyseAudio_PInfo.Core.Models;
using NAudio.CoreAudioApi;

namespace AnalyseAudio_PInfo.Models.Input
{
    public abstract class DeviceInput
    {
        protected DeviceInput() { }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsDefault { get; }
        public abstract AudioStream GetAudioStream();
        public abstract void Demarrer();
        public abstract void Arreter();


        public static void ScanSoundCards()
        {
            Logger.WriteLine("WASAPI Devices :");
            MMDeviceEnumerator enumerator = new();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All))
            {
                Logger.WriteLine($"\t{wasapi.DataFlow}\t{wasapi.FriendlyName}\t{wasapi.DeviceFriendlyName}\t{wasapi.State}");
            }
        }
    }
}
