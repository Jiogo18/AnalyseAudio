using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public class DeviceSpeaker : DeviceWasapi
    {
        private DeviceSpeaker(MMDevice wasapi, int index, DefaultSpeakers defaultSpeakers) : base(wasapi, index)
        {
            IsDefaultForCommunication = defaultSpeakers.Communication.ID == wasapi.ID;
            IsDefaultForConsole = defaultSpeakers.Console.ID == wasapi.ID;
            IsDefaultForMultimedia = defaultSpeakers.Multimedia.ID == wasapi.ID;
        }

        internal override bool IsDefaultForCommunication { get; }
        internal override bool IsDefaultForConsole { get; }
        internal override bool IsDefaultForMultimedia { get; }

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
}
