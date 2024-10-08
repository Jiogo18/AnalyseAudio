﻿using System.Threading.Tasks;

using AnalyseAudio_PInfo.Contracts.Services;
using AnalyseAudio_PInfo.Core.Helpers;

using Windows.Storage;

namespace AnalyseAudio_PInfo.Services
{
    public class LocalSettingsServicePackaged : ILocalSettingsService
    {
        public async Task<T> ReadSettingAsync<T>(string key)
        {
            object obj = null;

            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }

            return default;
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
    }
}
