using Microsoft.Windows.ApplicationModel.Resources;

namespace AnalyseAudio_PInfo.Helpers
{
    internal static class ResourceExtensions
    {
        private static ResourceLoader _resourceLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return _resourceLoader.GetString(resourceKey);
        }
    }
}
