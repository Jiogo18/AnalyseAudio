using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;

namespace AnalyseAudio_PInfo.Views.Keyboard
{
    public sealed partial class KeyboardNote : Grid, INotifyPropertyChanged
    {

        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note", typeof(int), typeof(Grid), new PropertyMetadata(69));
        public int Note
        {
            get { return (int)GetValue(NoteProperty); }
            set
            {
                if (Note == value) return;
                SetValue(NoteProperty, value);
                OnlyOneUpdate();
            }
        }
        public bool Black => IsBlack(Note);
        public int Octave => GetOctave(Note);
        public int NoteOfOctave => GetNoteOfOctave(Note);

        bool _pressed = false;
        public bool Pressed
        {
            get => _pressed;
            set { if (_pressed == value) return; _pressed = value; PressedChanged?.Invoke(this, Note); OnPropertyChanged(nameof(PressOpacity)); }
        }
        public event EventHandler<int> PressedChanged;

        private double PressOpacity => Pressed ? (Black ? 1 : 0.3) : 0;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }


        public KeyboardNote()
        {
            InitializeComponent();
        }

        public static int GetOctave(int note)
        {
            return note / 12;
        }

        public static int GetNoteOfOctave(int note)
        {
            return note % 12;
        }

        public static bool IsBlack(int note)
        {
            int noteOfOctave = GetNoteOfOctave(note);
            return noteOfOctave == 1 || noteOfOctave == 3 || noteOfOctave == 6 || noteOfOctave == 8 || noteOfOctave == 10;
        }

        public static double GetFrequencyFromNote(int note)
        {
            return 440 * Math.Pow(2, (note - 69) / 12.0);
        }

        public static double GetNoteFromFrequency(double freq)
        {
            return 12 * Math.Log(freq / 440) / Math.Log(2) + 69;
        }

        public static string GetNoteName(int note)
        {
            string[] gamme = { "do", "do#", "ré", "ré#", "mi", "fa", "fa#", "sol", "sol#", "la", "la#", "si" };
            return gamme[GetNoteOfOctave(note)] + " " + GetOctave(note);
        }

        void OnlyOneUpdate()
        {
            if (Black)
            {
                borderNote.Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 48));
                borderNote.CornerRadius = new CornerRadius(5);
                borderNote.Width = 26;
                borderNote.Height = 150;
                rectangleNote.Width = 26;
                double marginLeft = 13;
                double marginRight = 13;
                int noteOfOctave = NoteOfOctave;
                if (noteOfOctave == 1 || noteOfOctave == 6) marginLeft += 26;
                if (noteOfOctave == 3 || noteOfOctave == 10) marginRight += 26;
                boxNote.Margin = new Thickness { Top = 1, Bottom = 1, Left = marginLeft, Right = marginRight };
            }
        }

        private void Border_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pressed = true;
            OnlyOneUpdate();
        }

        private void Border_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Pressed = false;
        }
    }
}
