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
    }
}
