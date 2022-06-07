using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
	/// A Speaker using WASAPI. This allows you to record the computer's sound
	/// </summary>
    public class DeviceSpeaker : DeviceWasapi
    {
        private DeviceSpeaker(MMDevice wasapi, DefaultWasapi defaultSpeakers) : base(wasapi)
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

            DefaultWasapi defaultSpeakers = new(
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console),
                enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
                );
            List<DeviceSpeaker> devices = new();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active | DeviceState.Disabled | DeviceState.Unplugged))
                devices.Add(new DeviceSpeaker(wasapi, defaultSpeakers));
            return devices;
        }

        /// <summary>
		/// Start the recording of the speaker.
		/// </summary>
		/// A Speaker requires a WasapiLoopbackCapture, which is alomost the same as a WasapiCapture
		/// <param name="stream"></param>
		/// <param name="waveFormat"></param>
        public override void Start(AudioStream stream, WaveFormat waveFormat)
        {
            if (Recorder != null) return;
            Start(stream, new WasapiLoopbackCapture(wasapi), waveFormat);
        }
    }
}
