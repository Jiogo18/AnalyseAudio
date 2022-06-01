﻿using System.Threading.Tasks;

namespace AnalyseAudio_PInfo.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
