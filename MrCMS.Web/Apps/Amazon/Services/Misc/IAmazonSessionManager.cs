namespace MrCMS.Web.Apps.Amazon.Services.Misc
{
    public interface IAmazonSessionManager
    {
        T GetSessionValue<T>(string key, T defaultValue = default(T));
        void SetSessionValue<T>(string key, T item);
        void RemoveValue(string key);
    }
}