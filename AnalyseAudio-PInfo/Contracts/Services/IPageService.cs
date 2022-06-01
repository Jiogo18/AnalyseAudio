using System;

namespace AnalyseAudio_PInfo.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
