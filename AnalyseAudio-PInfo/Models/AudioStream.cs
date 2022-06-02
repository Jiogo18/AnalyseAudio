using System;

namespace AnalyseAudio_PInfo.Models
{
    public class AudioStream
    {
        // flux de double (amplitude)

        public AudioStream()
        {
            SampleRate = 0;
            SampleCount = 0;
        }

        public int SampleRate { get; }

        public int SampleCount { get; }

        public double[] Samples { get; set; }

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
