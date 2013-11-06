namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartSessionManager
    {
        T GetSessionValue<T>(string key, T defaultValue = default(T), bool encrypted = false);
        void SetSessionValue<T>(string key, T item, bool encrypt = false);
        void RemoveValue(string key);
    }
}