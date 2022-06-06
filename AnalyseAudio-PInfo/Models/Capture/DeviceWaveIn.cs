using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public class DeviceWaveIn : DeviceCapture
    {
        readonly int DeviceIndex;
        readonly WaveInCapabilities Capabilities;
        AudioStream Stream;
        public int SampleRate { get; private set; }
        private WaveInEvent Recorder;

        private DeviceWaveIn(int deviceIndex)
        {
            DeviceIndex = deviceIndex;
            Capabilities = WaveIn.GetCapabilities(deviceIndex);
        }

        public override string Name { get => Capabilities.ProductName; }
        public override string DisplayName { get => Name; }
        public override bool IsDefault { get => false; }
        public override bool IsRecording { get => Recorder != null; }
        bool Restart = false;
        public override WaveFormat WaveFormat
        {
            get => Recorder?.WaveFormat;
            set
            {
                if (Recorder == null) return;
                Restart = true;
                Recorder.StopRecording();
                Recorder.WaveFormat = value;
            }
        }

        public override void Start(AudioStream stream, WaveFormat waveFormat)
        {
            Stream = stream;
            SampleRate = 48000;
            Recorder = new WaveInEvent
            {
                BufferMilliseconds = 20, // 50 fps
                DeviceNumber = DeviceIndex,
                WaveFormat = waveFormat,
            };
            Recorder.DataAvailable += Wvin_DataAvailable;
            Recorder.RecordingStopped += Wvin_RecordingStopped;
            Recorder.StartRecording();
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

        private void Wvin_DataAvailable(object sender, WaveInEventArgs e)
        {
            Stream?.PushData(e.Buffer, e.BytesRecorded, SampleRate);
        }

        private void Wvin_RecordingStopped(object sender, StoppedEventArgs e)
        {
            Exception exception = e.Exception;
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

        public static List<DeviceWaveIn> ListDevices()
        {
            List<DeviceWaveIn> devices = new();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
                devices.Add(new DeviceWaveIn(i));
            return devices;
        }
    }
}
