using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Models.Capture;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace AnalyseAudio_PInfo.ViewModels
{
    public class CaptureViewModel : ObservableRecipient
    {
        public readonly CaptureManager Capture;
        public readonly DeviceCaptureManager Devices;
        readonly List<string> ModifiedProperties = new();
        public bool IsAutoUpdate
        {
            get => Devices.IsAutoUpdate;
            set { Devices.IsAutoUpdate = value; if (value) Update(); }
        }

        public CaptureViewModel()
        {
            Capture = Manager.Capture;
            Devices = Manager.DeviceCapture;
            Devices.RestoreSelection();
        }

        public void StartStop()
        {
            if (Capture.State == CaptureStatus.Stopped)
                Capture.Start();
            else
                Capture.Stop();
        }

        // Update the Capture manager
        public void Update()
        {
            if (Capture == null) return;
            ModifiedProperties.ForEach(UpdateProperty);
            ModifiedProperties.Clear();
            Devices.ApplyChanges();
        }

        void UpdateProperty(string propertyName)
        {
            if (Capture == null) return;
        }
    }
}
