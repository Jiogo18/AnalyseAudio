using System;

namespace AnalyseAudio_PInfo.Models.Capture
{
    public abstract class DeviceCapture
    {
        protected DeviceCapture() { }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsDefault { get; }
        public abstract void Start(AudioStream stream);
        public abstract void Stop();

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

        public enum StoppedReason
        {
            Error,
            External,
            Unknown
        };
    }
}
