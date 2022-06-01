using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Input
{
    public class DeviceSpeaker : DeviceInput
    {
        readonly MMDevice wasapi;
        private DeviceSpeaker(MMDevice wasapiDevice, int index, DefaultSpeakers defaultSpeakers)
        {
            wasapi = wasapiDevice;
            Name = wasapi.FriendlyName;
            IsDefaultForCommunication = defaultSpeakers.Communication.ID == wasapi.ID;
            IsDefaultForConsole = defaultSpeakers.Console.ID == wasapi.ID;
            IsDefaultForMultimedia = defaultSpeakers.Multimedia.ID == wasapi.ID;
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


        public bool Equals(DeviceSpeaker that)
        {
            return wasapi.ID == that.wasapi.ID;
        }

        public static List<DeviceSpeaker> ListWASAPI()
        {
            MMDeviceEnumerator enumerator = new();

            DefaultSpeakers defaultSpeakers = new(
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
                );
            List<DeviceSpeaker> devices = new();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                devices.Add(new DeviceSpeaker(wasapi, devices.Count, defaultSpeakers));
            enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All);
            return devices;
        }
    }

    struct DefaultSpeakers
    {
        internal readonly MMDevice Communication;
        internal readonly MMDevice Console;
        internal readonly MMDevice Multimedia;

        public DefaultSpeakers(MMDevice communication, MMDevice console, MMDevice multimedia)
        {
            Communication = communication;
            Console = console;
            Multimedia = multimedia;
        }
    }
}
