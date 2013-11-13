using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using NHibernate;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Services.SagePay
{
    public class SagePayCartLoader : ISagePayCartLoader
    {
        private readonly ISession _session;
        private readonly ICartBuilder _cartBuilder;

        public SagePayCartLoader(ISession session, ICartBuilder cartBuilder)
        {
            _session = session;
            _cartBuilder = cartBuilder;
        }

        public CartModel GetCart(string vendorTxCode)
        {
            var serializedTxCode = JsonConvert.SerializeObject(vendorTxCode);
            var sessionData = _session.QueryOver<SessionData>().Where(data => data.Key == CartManager.CurrentCartGuid && data.Data == serializedTxCode).SingleOrDefault();
            return sessionData == null
                       ? null
                       : _cartBuilder.BuildCart(sessionData.UserGuid);
        }
    }
}