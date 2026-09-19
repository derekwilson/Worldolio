namespace WorldolioMauiPOC.Utility
{
    public static class ResourceExtensions
    {
        public static T GetResource<T>(this ResourceDictionary dictionary, string key, T defaultValue)
        {
            if (dictionary.TryGetValue(key, out var value) && value is T resource)
            {
                return resource;
            }
            return defaultValue;
        }
    }
}
