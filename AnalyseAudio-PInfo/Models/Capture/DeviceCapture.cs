using AnalyseAudio_PInfo.Core.Models;
using NAudio.CoreAudioApi;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public abstract class DeviceCapture
    {
        protected DeviceCapture() { }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsDefault { get; }
        public abstract void Start(AudioStream stream);
        public abstract void Stop();


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
