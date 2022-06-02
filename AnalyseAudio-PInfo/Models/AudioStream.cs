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

        public void PushData(byte[] data, int length)
        {
            // TODO : FFT here ? (with a timer)
            DataAvailable?.Invoke(this, new DataReceivedEventArgs(data, length)); // DataAvailable null ? voir comment c'est fait ailleur
        }

        public event EventHandler<DataReceivedEventArgs> DataAvailable;
    }

    public class DataReceivedEventArgs : EventArgs
    {
        public readonly byte[] Data;
        public readonly int Length;

        internal DataReceivedEventArgs(byte[] data, int length)
        {
            Data = data;
            Length = length;
        }
    }

}
