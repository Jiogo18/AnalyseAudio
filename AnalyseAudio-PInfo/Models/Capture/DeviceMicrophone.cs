using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
	/// A Microphone using WASAPI
	/// </summary>
    public class DeviceMicrophone : DeviceWasapi
    {
        private DeviceMicrophone(MMDevice wasapi, DefaultWasapi defaultMicrophones) : base(wasapi)
        {
            IsDefaultForCommunication = defaultMicrophones.Communication?.ID == wasapi.ID;
            IsDefaultForConsole = defaultMicrophones.Console?.ID == wasapi.ID;
            IsDefaultForMultimedia = defaultMicrophones.Multimedia?.ID == wasapi.ID;
        }

        internal override bool IsDefaultForCommunication { get; }
        internal override bool IsDefaultForConsole { get; }
        internal override bool IsDefaultForMultimedia { get; }

        public static List<DeviceMicrophone> ListWASAPI()
        {
            MMDeviceEnumerator enumerator = new();
            DefaultWasapi defaultMicrophones = new(
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia)
                );
            List<DeviceMicrophone> devices = new();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active | DeviceState.Disabled | DeviceState.Unplugged))
                devices.Add(new DeviceMicrophone(wasapi, defaultMicrophones));
            return devices;
        }
    }
}
