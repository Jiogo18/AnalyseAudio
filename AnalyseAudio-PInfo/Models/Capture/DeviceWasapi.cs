using AnalyseAudio_PInfo.Models.Common;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Windows.Forms;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
    /// WASAPI is a category of devices/connections.
    /// It allows computer's recording (DeviceSpeaker) and is pretty stable.
    /// </summary>
    public abstract class DeviceWasapi : DeviceCapture
    {
        protected readonly MMDevice wasapi;
        protected WasapiCapture Recorder { get; private set; }
        internal AudioStream Stream { get; set; }

        internal DeviceWasapi(MMDevice wasapiDevice)
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
                // Prefix of the Display name, depending on the status of the device
                string Prefix = wasapi.State switch
                {
                    DeviceState.Active => "✔️",
                    DeviceState.Disabled => "🔇",
                    DeviceState.Unplugged => "🔌",
                    DeviceState.NotPresent => "🚫",
                    _ => "❌"
                };
                // Suffix of the Display name, depending on the default devices
                string Suffix =
                    (IsDefaultForCommunication ? "📞" : "") + // Default for Communication: call, phone, meeting...
                    (IsDefaultForConsole ? "📟" : "") +// Default for Console: System, games, main applications...
                    (IsDefaultForMultimedia ? "🎶" : "");// Default for Multimedia: Music, videos, movies...
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
            // We have to wait for the Recorder to call Recorder_RecordingStopped and restarting it.
        }


        public override void Start(AudioStream stream, WaveFormat waveFormat)
        {
            if (Recorder != null) return;
            Start(stream, new WasapiCapture(wasapi), waveFormat);
        }

        /// <summary>
        /// Start the record for all Wasapi devices
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="recorder"></param>
        /// <param name="waveFormat"></param>
        protected void Start(AudioStream stream, WasapiCapture recorder, WaveFormat waveFormat)
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
                // Stopped normally
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

    /// <summary>
    /// 3 devices (different or not) used by DeviceMicrophone and DeviceSpeaker constructors.
    /// </summary>
    struct DefaultWasapi
    {
        internal readonly MMDevice Communication;
        internal readonly MMDevice Console;
        internal readonly MMDevice Multimedia;

        public DefaultWasapi(MMDevice communication, MMDevice console, MMDevice multimedia)
        {
            Communication = communication;
            Console = console;
            Multimedia = multimedia;
        }
    }
}
