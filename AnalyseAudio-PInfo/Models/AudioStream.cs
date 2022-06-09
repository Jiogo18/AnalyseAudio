using System;

namespace AnalyseAudio_PInfo.Models
{
    /// <summary>
    /// A Bridge between a DeviceCapture and the SpectrogramGenerator
    /// </summary>
    public class AudioStream
    {
        public AudioStream() { }

        public void PushData(byte[] data, int length, int sampleRate)
        {
            DataAvailable?.Invoke(this, new DataReceivedEventArgs(data, length, sampleRate));
        }

        public event EventHandler<DataReceivedEventArgs> DataAvailable;
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public readonly byte[] Data;
        public readonly int Length;
        public readonly int SampleRate;

        internal DataReceivedEventArgs(byte[] data, int length, int sampleRate)
        {
            Data = data;
            Length = length;
            SampleRate = sampleRate;
        }
    }

}
