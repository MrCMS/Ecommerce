using System.Collections.Generic;
using System.Collections.Specialized;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Paypoint;
using NHibernate;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.Paypoint
{
    public class PaypointResponseLogger : IPaypointResponseLogger
    {
        private readonly CartModel _cart;
        private readonly ISession _session;

        public PaypointResponseLogger(CartModel cart, ISession session)
        {
            _cart = cart;
            _session = session;
        }

        public void LogResponse(string rawData, NameValueCollection collection)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var allKey in collection.AllKeys)
            {
                dictionary[allKey] = collection[allKey];
            }
            _session.Transact(session =>
            {
                var paypointResponse = new PaypointResponse
                {
                    RawData = rawData,
                    Response = JsonConvert.SerializeObject(dictionary)
                };
                paypointResponse.SetGuid(_cart.CartGuid);
                return session.Save(paypointResponse);
            });
        }
    }
}