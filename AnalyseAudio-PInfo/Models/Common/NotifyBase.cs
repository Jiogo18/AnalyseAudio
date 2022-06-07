using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnalyseAudio_PInfo.Models.Common
{
    /// <summary>
	/// A Class to provide OnPropertyChanged in order to send the update event of properties
	/// </summary>
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}