using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PaypalExpressCartLoader : IPaypalExpressCartLoader
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ISession _session;

        public PaypalExpressCartLoader(ISession session, ICartBuilder cartBuilder)
        {
            _session = session;
            _cartBuilder = cartBuilder;
        }

        public CartModel GetCart(string token)
        {
            string serializedTxCode = JsonConvert.SerializeObject(token);
            SessionData sessionData =
                _session.QueryOver<SessionData>()
                    .Where(data => data.Key == CartManager.CurrentPayPalExpressToken && data.Data == serializedTxCode)
                    .SingleOrDefault();
            return sessionData == null
                ? null
                : _cartBuilder.BuildCart(sessionData.UserGuid);
        }
    }
}