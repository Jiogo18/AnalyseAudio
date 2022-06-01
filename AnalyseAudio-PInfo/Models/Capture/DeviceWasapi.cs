using AnalyseAudio_PInfo.Core.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public abstract class DeviceWasapi : DeviceCapture
    {
        readonly MMDevice wasapi;
        WasapiCapture recorder;
        internal DeviceWasapi(MMDevice wasapiDevice, int index)
        {
            wasapi = wasapiDevice;
            Name = wasapi.FriendlyName;
        }

        ~DeviceWasapi()
        {
            wasapi.Dispose();
            if (recorder != null)
            {
                recorder.StopRecording();
                recorder.Dispose();
                recorder = null;
            }
        }

        public override string Name { get; }
        public override string DisplayName { get => Name + " " + (IsDefaultForCommunication ? "📞" : "") + (IsDefaultForConsole ? "📟" : "") + (IsDefaultForMultimedia ? "🎶" : ""); }
        public override bool IsDefault => IsDefaultForConsole;
        internal abstract bool IsDefaultForCommunication { get; }
        internal abstract bool IsDefaultForConsole { get; }
        internal abstract bool IsDefaultForMultimedia { get; }

        private AudioStream Stream;
        public override void Start(AudioStream stream)
        {
            if (recorder != null) return;

            try
            {
                recorder = new(wasapi);
                recorder.ShareMode = AudioClientShareMode.Shared;
                recorder.DataAvailable += RecorderDataAvailable;
                Stream = stream;
                var inprov = new WaveInProvider(recorder);
                var fto16prov = new WaveFloatTo16Provider(inprov);
                var stomprov = new StereoToMonoProvider16(fto16prov);

                recorder.StartRecording();
            }
            catch (Exception e)
            {
                Logger.WriteLine("!!! ERROR !!!" +
                    "\nMessage:\n   " + e.Message +
                    "\nSource:\n   " + e.Source +
                    "\nStack:\n" + e.StackTrace);
            }
        }

        public override void Stop()
        {
            if (recorder == null) return;
            recorder.StopRecording();
            recorder.Dispose();
            recorder = null;
            Stream = null;
        }

        public bool Equals(DeviceWasapi that)
        {
            return wasapi.ID == that.wasapi.ID;
        }

        private void RecorderDataAvailable(object sender, WaveInEventArgs e)
        {
            Logger.WriteLine($"BytesRecorded: {e.BytesRecorded}");
            Stream?.PushData(e.Buffer, e.BytesRecorded);
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
