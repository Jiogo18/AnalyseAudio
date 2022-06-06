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
        internal AudioStream Stream { get; set; }

        internal DeviceWasapi(MMDevice wasapiDevice, int index)
        {
            wasapi = wasapiDevice;
            Name = wasapi.FriendlyName;
        }

        ~DeviceWasapi()
        {
            if (Recorder != null)
            {
                Recorder.StopRecording();
                Recorder.Dispose();
                Recorder = null;
            }
            wasapi.Dispose();
        }

        public override string Name { get; }
        public override string DisplayName
        {
            get
            {
                string Prefix = wasapi.State switch
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
        public override bool IsRecording => Recorder != null;
        bool Restart = false;
        public override WaveFormat WaveFormat
        {
            get => Recorder?.WaveFormat;
            set { if (Recorder == null) return; Restart = true; Recorder.StopRecording(); Recorder.WaveFormat = value; }
        }


        public override void Start(AudioStream stream, WaveFormat waveFormat)
        {
            if (Recorder != null) return;
            Start(stream, new WasapiCapture(wasapi), waveFormat);
        }

        internal void Start(AudioStream stream, WasapiCapture recorder, WaveFormat waveFormat)
        {
            if (Recorder != null) return;

            try
            {
                Recorder = recorder;
                Recorder.ShareMode = AudioClientShareMode.Shared;
                Recorder.RecordingStopped += Recorder_RecordingStopped;
                Recorder.DataAvailable += RecorderDataAvailable;

                Recorder.WaveFormat = waveFormat;

                Stream = stream;

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
            StopInternal();
            Stopped(StoppedReason.External);
        }

        protected override void StopInternal()
        {
            if (Recorder != null)
            {
                Recorder.StopRecording();
                Recorder.Dispose();
                Recorder = null;
            }
            Stream = null;
        }

        private void RecorderDataAvailable(object sender, WaveInEventArgs e)
        {
            Stream?.PushData(e.Buffer, e.BytesRecorded, Recorder.WaveFormat.SampleRate);
        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception == null)
            {
                if (Restart)
                {
                    Restart = false;
                    try
                    {
                        Recorder.StartRecording();
                    }
                    catch (Exception e2)
                    {
                        StopByException(e2);
                    }
                }
            }
            else
            {
                StopByException(e.Exception);
            }
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
