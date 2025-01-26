using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AnalyseAudio.Models.Spectro
{
    internal class FrequencyScaleGenerator
    {
        public static Bitmap GenerateFrequencyScaleBitmap(double minFreq, double maxFreq, int width, int height, int margin)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Font labelFont = new Font("Arial", 12);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                double range = maxFreq - minFreq;
                // Step size for the ticks
                double stepSize = CalculateStepSize(range, 4, 10);
                // Floor minFreq modulo stepSize
                double baseFreq = minFreq - (minFreq % stepSize);

                for (double freq = baseFreq; freq < maxFreq; freq += stepSize)
                {
                    double yPos = height - (freq - minFreq) / range * height;
                    if (yPos >= margin && yPos <= height - margin)
                    {
                        g.DrawLine(Pens.Black, 0, (float)yPos, 10, (float)yPos);
                        g.DrawString(freq.ToString(), labelFont, Brushes.Black, new PointF(15, (float)yPos - 10));
                    }
                }
            }

            return bitmap;
        }

        private static double CalculateStepSize(double range, int minTicks, int maxTicks)
        {
            double rawStepSize = range / maxTicks;
            // The power of 10 below rawStepSize
            double magnitude = Math.Pow(10, Math.Floor(Math.Log10(rawStepSize)));
            double residual = rawStepSize / magnitude;

            if (residual > 5)
            {
                return 10 * magnitude;
            }
            else if (residual > 2)
            {
                return 5 * magnitude;
            }
            else if (residual > 1)
            {
                return 2 * magnitude;
            }
            else
            {
                return magnitude;
            }
        }
    }

}
