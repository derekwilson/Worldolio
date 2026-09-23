using Microsoft.Maui.Graphics.Platform;
using System.Reflection;
using Worldolio.Data.Logging;

namespace WorldolioMauiPOC.Utility
{
    public interface IResourceProvider
    {
        public TYPE GetResource<TYPE>(string name, TYPE defaultValue);
        public Microsoft.Maui.Graphics.IImage? LoadImageFromEmbeddedResource(string path);
    }

    public class ResourceProvider : IResourceProvider
    {
        private ILogger _logger;

        public ResourceProvider(ILogger logger)
        {
            _logger = logger;
        }

        public TYPE GetResource<TYPE>(string name, TYPE defaultValue)
        {
            if (Application.Current == null)
            {
                _logger.Warning(() => $"ResourceProvider.GetResource cannot find {name}, returning default");
                return defaultValue;
            }
            return Application.Current.Resources.GetResource<TYPE>(name, defaultValue);
        }

        public Microsoft.Maui.Graphics.IImage? LoadImageFromEmbeddedResource(string path)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream? stream = assembly.GetManifestResourceStream(path))
            {
                try
                {
                    return PlatformImage.FromStream(stream);
                } 
                catch(Exception ex)
                {
                    _logger.LogException(() => $"ResourceProvider.LoadImageFromEmbeddedResource cannot load {path}", ex);
                }
            }

            _logger.Warning(() => $"ResourceProvider.LoadImageFromEmbeddedResource cannot load {path}, returning null");
            return null;
        }
    }
}
