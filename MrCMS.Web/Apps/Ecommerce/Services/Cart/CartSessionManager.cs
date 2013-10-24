using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Ecommerce.Entities;
using NHibernate;
using Newtonsoft.Json;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class CartSessionManager : ICartSessionManager
    {
        private readonly ISession _session;
        private readonly IGetUserGuid _getUserGuid;
        private readonly Site _site;

        public CartSessionManager(ISession session, IGetUserGuid getUserGuid, Site site)
        {
            _session = session;
            _getUserGuid = getUserGuid;
            _site = site;
        }

        public T GetSessionValue<T>(string key, T defaultValue = default(T))
        {
            var sessionData =
                _session.QueryOver<SessionData>()
                        .Where(data => data.UserGuid == _getUserGuid.UserGuid && data.Site == _site && data.Key == key)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();
            if (sessionData == null)
                return defaultValue;
            try
            {
                return JsonConvert.DeserializeObject<T>(sessionData.Data);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void SetSessionValue<T>(string key, T item)
        {
            var sessionData =
                _session.QueryOver<SessionData>()
                        .Where(data => data.UserGuid == _getUserGuid.UserGuid && data.Site == _site && data.Key == key)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault() ?? new SessionData { Key = key, UserGuid = _getUserGuid.UserGuid };

            sessionData.Data = JsonConvert.SerializeObject(item);
            _session.Transact(session => session.SaveOrUpdate(sessionData));
        }

        public void RemoveValue(string key)
        {
            var sessionData =
                _session.QueryOver<SessionData>()
                        .Where(data => data.UserGuid == _getUserGuid.UserGuid && data.Site == _site && data.Key == key)
                        .Take(1)
                        .Cacheable()
                        .SingleOrDefault();
            if (sessionData != null)
                _session.Transact(session => session.Delete(sessionData));
        }
    }
}