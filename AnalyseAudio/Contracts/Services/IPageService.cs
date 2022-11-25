using System;

namespace AnalyseAudio.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);
    }
}
