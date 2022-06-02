using AnalyseAudio_PInfo.Core.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Windows.Forms;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public abstract class DeviceWasapi : DeviceCapture
    {
        protected readonly MMDevice wasapi;
        protected WasapiCapture Recorder { get; private set; }
        WaveInProvider inprov;
        WaveFloatTo16Provider fto16prov;
        StereoToMonoProvider16 stomprov;
        internal DeviceWasapi(MMDevice wasapiDevice, int index)
        {
            wasapi = wasapiDevice;
            Name = wasapi.FriendlyName;
        }

        ~DeviceWasapi()
        {
            wasapi.Dispose();
            if (Recorder != null)
            {
                Recorder.StopRecording();
                Recorder.Dispose();
                Recorder = null;
            }
        }

        public override string Name { get; }
        public override string DisplayName
        {
            get
            {
                string Prefix = (wasapi.State) switch
                {
                    DeviceState.Active => "✔️",
                    DeviceState.Disabled => "🔇",
                    DeviceState.Unplugged => "🔌",
                    DeviceState.NotPresent => "🚫",
                    _ => "❌"
                };
                string Suffix =
                    (IsDefaultForCommunication ? "📞" : "") +
                    (IsDefaultForConsole ? "📟" : "") +
                    (IsDefaultForMultimedia ? "🎶" : "");
                return $"{Prefix} {Name} {Suffix}";
            }
        }
        public override bool IsDefault => IsDefaultForConsole;
        public DeviceState State => wasapi.State;
        internal abstract bool IsDefaultForCommunication { get; }
        internal abstract bool IsDefaultForConsole { get; }
        internal abstract bool IsDefaultForMultimedia { get; }
        protected bool IsRecording => Recorder != null;

        internal AudioStream Stream { get; set; }
        public override void Start(AudioStream stream)
        {
            if (Recorder != null) return;
            Start(stream, new WasapiCapture(wasapi));
        }

        internal void Start(AudioStream stream, WasapiCapture recorder)
        {
            if (Recorder != null) return;

            try
            {
                Recorder = recorder;
                Recorder.ShareMode = AudioClientShareMode.Shared;
                Recorder.RecordingStopped += Recorder_RecordingStopped;
                Recorder.DataAvailable += RecorderDataAvailable;
                Stream = stream;
                inprov = new(Recorder);
                fto16prov = new(inprov);
                stomprov = new(fto16prov);

                Recorder.StartRecording();
                Started();
            }
            catch (Exception e)
            {
                Logger.Error("!!! ERROR !!!" +
                    "\nMessage:\n   " + e.Message +
                    "\nSource:\n   " + e.Source +
                    "\nStack:\n" + e.StackTrace);
                MessageBox.Show($"Error while recording the device {DisplayName}: {e.Message} (from {e.Source})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                recorder.StopRecording();
                recorder.Dispose();
                recorder = null;
                Stopped(StoppedReason.Error);
            }
        }

        public override void Stop()
        {
            if (Recorder == null) return;
            Recorder.StopRecording();
            Recorder.Dispose();
            Recorder = null;
            Stream = null;
            Stopped(StoppedReason.External);
        }

        public bool Equals(DeviceWasapi that)
        {
            return wasapi.ID == that.wasapi.ID;
        }

        private void RecorderDataAvailable(object sender, WaveInEventArgs e)
        {
            Stream?.PushData(e.Buffer, e.BytesRecorded);
            byte[] buffer = new byte[e.BytesRecorded];
            // Read one of these providers to empty the buffer
            inprov.Read(buffer, 0, e.BytesRecorded);
            //fto16prov.Read(buffer, 0, e.BytesRecorded);
            //stomprov.Read(buffer, 0, e.BytesRecorded);
        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (Recorder == null) return;
            Recorder.Dispose();
            Recorder = null;
            Stopped(StoppedReason.Unknown);
            Logger.Warn($"Recorder suddenly stopped recording:\n{e.Exception}");
        }

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
