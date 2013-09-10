using System;
using System.Web;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Amazon.Services.Misc
{
    public class AmazonSessionManager : IAmazonSessionManager
    {
        private readonly HttpSessionStateBase _session;

        public AmazonSessionManager(HttpSessionStateBase session)
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

        public void RemoveValue(string key)
        {
            if (_session != null)
                _session.Remove(key);
        }
    }
}