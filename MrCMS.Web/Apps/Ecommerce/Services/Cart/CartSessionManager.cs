using System;
using System.Web;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartSessionManager : ICartSessionManager
    {
        private readonly HttpSessionStateBase _session;

        public CartSessionManager(HttpSessionStateBase session)
        {
            _session = session;
        }

        public T GetSessionValue<T>(string key, T defaultValue = default(T))
        {
            if (_session != null)
            {
                try { return (T)Convert.ChangeType(_session[key], typeof(T)); }
                catch { }
            }
            return defaultValue;
        }

        public void SetSessionValue<T>(string key, T item)
        {
            if (_session != null)
                _session[key] = item;
        }
    }
}