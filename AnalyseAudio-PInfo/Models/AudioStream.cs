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
            // TODO
        }
    }
}
