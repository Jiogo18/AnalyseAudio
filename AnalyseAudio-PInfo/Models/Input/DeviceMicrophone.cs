using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Input
{
    public class DeviceMicrophone : DeviceInput
    {
        readonly MMDevice wasapi;
        private DeviceMicrophone(MMDevice wasapiDevice, int index, DefaultMicrophones defaultMicrophones)
        {
            wasapi = wasapiDevice;
            Name = wasapi.FriendlyName;
            IsDefaultForCommunication = defaultMicrophones.Communication.ID == wasapi.ID;
            IsDefaultForConsole = defaultMicrophones.Console.ID == wasapi.ID;
            IsDefaultForMultimedia = defaultMicrophones.Multimedia.ID == wasapi.ID;
        }

        public override string Name { get; }
        public override string DisplayName { get => Name + " " + (IsDefaultForCommunication ? "📞" : "") + (IsDefaultForConsole ? "📟" : "") + (IsDefaultForMultimedia ? "🎶" : ""); }
        public override bool IsDefault => IsDefaultForConsole;
        readonly bool IsDefaultForCommunication;
        readonly bool IsDefaultForConsole;
        readonly bool IsDefaultForMultimedia;

        public override AudioStream GetAudioStream()
        {
            return null;
        }

        public override void Demarrer() { }
        public override void Arreter() { }

        public bool Equals(DeviceMicrophone that)
        {
            return wasapi.ID == that.wasapi.ID;
        }

        public static List<DeviceMicrophone> ListWASAPI()
        {
            MMDeviceEnumerator enumerator = new();
            DefaultMicrophones defaultMicrophones = new(
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia)
                );
            List<DeviceMicrophone> devices = new();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
                devices.Add(new DeviceMicrophone(wasapi, devices.Count, defaultMicrophones));
            enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All);
            return devices;
        }
    }

    struct DefaultMicrophones
    {
        internal readonly MMDevice Communication;
        internal readonly MMDevice Console;
        internal readonly MMDevice Multimedia;

        public DefaultMicrophones(MMDevice communication, MMDevice console, MMDevice multimedia)
        {
            Communication = communication;
            Console = console;
            Multimedia = multimedia;
        }
    }
}
