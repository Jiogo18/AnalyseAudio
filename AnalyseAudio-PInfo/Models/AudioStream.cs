using System;

namespace AnalyseAudio_PInfo.Models
{
    public class AudioStream
    {
        // flux de double (amplitude)

        public AudioStream(int sampleRate, int channelCount, int sampleCount)
        {
            SampleRate = sampleRate;
            ChannelCount = channelCount;
            SampleCount = sampleCount;
        }

        public int SampleRate { get; }

        public int ChannelCount { get; }

        public int SampleCount { get; }

        public double[] Samples { get; set; }

        public double[] GetChannel(int channel)
        {
            if (channel < 0 || channel >= ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(channel));

            var samples = new double[SampleCount];
            for (var i = 0; i < SampleCount; i++)
                samples[i] = Samples[i * ChannelCount + channel];
            return samples;
        }

        public void SetChannel(int channel, double[] samples)
        {
            if (channel < 0 || channel >= ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(channel));

            if (samples.Length != SampleCount)
                throw new ArgumentException("The number of samples must be equal to the number of samples in the stream.", nameof(samples));

            for (var i = 0; i < SampleCount; i++)
                Samples[i * ChannelCount + channel] = samples[i];
        }
    }
}
