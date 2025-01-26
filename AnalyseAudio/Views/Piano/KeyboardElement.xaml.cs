using AnalyseAudio.Models.Common;
using Microsoft.UI.Xaml.Controls;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace AnalyseAudio.Views.Keyboard
{
    public sealed partial class KeyboardElement : Grid
    {
        readonly IDictionary<int, WaveOutEvent> wavesOut = new Dictionary<int, WaveOutEvent>();
        public readonly SignalGeneratorType[] WavesType = Enum.GetValues<SignalGeneratorType>();
        SignalGeneratorType WaveType { get; set; } = SignalGeneratorType.Sin;
        double Gain { get; set; } = 0.02;

        public KeyboardElement()
        {
            InitializeComponent();

            for (int note = 60; note < 89; note++)
            {
                KeyboardNote keyboardNote = new() { Note = note };
                if (keyboardNote.Black)
                {
                    Blacks.Children.Add(keyboardNote);
                }
                else
                {
                    Whites.Children.Add(keyboardNote);
                }
                keyboardNote.PressedChanged += KeyboardNote_PressedChanged;
            }
        }

        ~KeyboardElement()
        {
            foreach (KeyValuePair<int, WaveOutEvent> kvp in wavesOut)
            {
                kvp.Value.Stop();
                kvp.Value.Dispose();
            }
            wavesOut.Clear();
        }

        void Play(int note, double frequency)
        {
            if (wavesOut.ContainsKey(note)) return;

            var signalGenerator = new SignalGenerator()
            {
                Gain = Gain,
                Frequency = frequency,
                Type = WaveType,
            };

            var wo = new WaveOutEvent();
            wo.Init(signalGenerator);
            wo.Play();
            wavesOut.Add(note, wo);
            Logger.WriteLine($"Piano {note} ({frequency} Hz) playing");
        }

        void End(int note, double frequency)
        {
            if (wavesOut.TryGetValue(note, out WaveOutEvent wo))
            {
                wo.Stop();
                wo.Dispose();
                wavesOut.Remove(note);
            }
            Logger.WriteLine($"Piano {note} ({frequency} Hz) stopped");
        }

        private void KeyboardNote_PressedChanged(object sender, int note)
        {
            double frequency = KeyboardNote.GetFrequencyFromNote(note);
            if ((sender as KeyboardNote).Pressed)
            {
                Play(note, frequency);
            }
            else
            {
                End(note, frequency);
            }
        }
    }
}
