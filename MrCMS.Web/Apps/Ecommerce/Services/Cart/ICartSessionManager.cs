using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface ICartSessionManager
    {
        T GetSessionValue<T>(string key, Guid userGuid, T defaultValue = default(T), bool encrypted = false);
        void SetSessionValue<T>(string key, Guid userGuid, T item, TimeSpan? expireIn = null, bool encrypt = false);
        void RemoveValue(string key, Guid userGuid);
    }
}