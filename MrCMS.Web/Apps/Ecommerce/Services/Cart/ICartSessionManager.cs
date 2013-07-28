namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartSessionManager
    {
        T GetSessionValue<T>(string key, T defaultValue = default(T));
        void SetSessionValue<T>(string key, T item);
    }
}