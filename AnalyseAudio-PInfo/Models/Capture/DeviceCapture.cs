using AnalyseAudio_PInfo.Core.Models;
using NAudio.Wave;
using System;

namespace AnalyseAudio_PInfo.Models.Capture
{
    /// <summary>
	/// An abstract Device to record audio
	/// </summary>
    public abstract class DeviceCapture
    {
        protected DeviceCapture() { }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsDefault { get; }
        public abstract bool IsRecording { get; }
        public abstract WaveFormat WaveFormat { get; set; }
        
        public event EventHandler OnStart;
        public event EventHandler<StoppedReason> OnStop;

        /// <summary>
		/// Start the recording with this device
		/// </summary>
		/// <param name="stream"></param> The stream where you push the datas
		/// <param name="waveFormat"></param> The format used for the stream
        public abstract void Start(AudioStream stream, WaveFormat waveFormat);
        /// <summary>
		/// Stop the recording of this device
		/// </summary>
        public abstract void Stop();
        /// <summary>
		/// End the recording of this device, without triggering 'OnStop' event
		/// </summary>
        protected abstract void StopInternal();

        protected void Started()
        {
            OnStart?.Invoke(this, null);
        }

        protected void Stopped(StoppedReason reason)
        {
            OnStop?.Invoke(this, reason);
        }

        /// <summary>
        /// An Exception occurend and the record has to stop
        /// </summary>
        /// <param name="e"></param>
        protected void StopByException(Exception e)
        {
            Logger.Error($"Recorder suddenly stopped recording:\n{e}");
            StopInternal();
            Stopped(StoppedReason.Unknown);
        }

        /// <summary>
		/// Reason used when the recording stops
		/// Error: Because an error occured
		/// External: Because Stop() was trigerred
		/// Unknown: Unknown exception occured
		/// </summary>
        public enum StoppedReason
        {
            Error,
            External,
            Unknown
        };
    }
}
