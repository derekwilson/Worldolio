namespace WorldolioMauiPOC.Utility
{
    public interface IResourceHelper
    {
        public TYPE GetResource<TYPE>(string name, TYPE defaultValue);
    }

    public class ResourceHelper : IResourceHelper
    {
        public TYPE GetResource<TYPE>(string name, TYPE defaultValue)
        {
            if (Application.Current == null)
            {
                return defaultValue;
            }
            return Application.Current.Resources.GetResource<TYPE>(name, defaultValue);
        }
    }
}
