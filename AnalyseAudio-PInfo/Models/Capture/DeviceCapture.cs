using AnalyseAudio_PInfo.Core.Models;
using NAudio.Wave;
using System;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public abstract class DeviceCapture
    {
        protected DeviceCapture() { }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsDefault { get; }
        public abstract bool IsRecording { get; }
        public abstract WaveFormat WaveFormat { get; set; }
        public abstract void Start(AudioStream stream, WaveFormat waveFormat);
        public abstract void Stop();
        protected abstract void StopInternal();

        public event EventHandler OnStart;
        public event EventHandler<StoppedReason> OnStop;

        protected void Started()
        {
            OnStart?.Invoke(this, null);
        }

        protected void Stopped(StoppedReason reason)
        {
            OnStop?.Invoke(this, reason);
        }

        protected void StopByException(Exception e)
        {
            Logger.Error($"Recorder suddenly stopped recording:\n{e}");
            StopInternal();
            Stopped(StoppedReason.Unknown);
        }

        public enum StoppedReason
        {
            Error,
            External,
            Unknown
        };
    }
}
