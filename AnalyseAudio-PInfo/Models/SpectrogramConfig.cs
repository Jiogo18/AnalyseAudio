namespace AnalyseAudio_PInfo.Models
{
    /// <summary>
	/// Configuration for a Spectrogram.SpectrogramGenerator.
	/// </summary>
    public class SpectrogramConfig
    {
        public int SampleRate = 48000;
        public int FFTSize = 4096;
        public int StepSize = 100;
        public double FreqMin = 0;
        public double FreqMax = 3000;
        public int OffsetHz = 0;

        /// <summary>
		/// Create a configuration for a generator
		/// </summary>
		/// <param name="generator"></param>
        internal SpectrogramConfig(Spectrogram.SpectrogramGenerator generator = null)
        {
            if (generator == null) return;
            SampleRate = generator.SampleRate;
            FFTSize = generator.FftSize;
            StepSize = generator.StepSize;
            FreqMin = generator.FreqMin;
            FreqMax = generator.FreqMax;
            OffsetHz = generator.OffsetHz;
        }

        /// <summary>
		/// Create a generator with this configuration
		/// </summary>
		/// <returns></returns> A new Spectrogram.SpectrogramGenerator
        internal Spectrogram.SpectrogramGenerator CreateGenerator()
        {
            return new Spectrogram.SpectrogramGenerator(SampleRate, FFTSize, StepSize, FreqMin, FreqMax, offsetHz: OffsetHz);
        }
    }
}
