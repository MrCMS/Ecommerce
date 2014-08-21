using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IGetPaypalShippingOptions
    {
        List<PaypalShippingOption> Get(PaypalShippingInfo info);
    }

    public class GetPaypalShippingOptions : IGetPaypalShippingOptions
    {
        private readonly IPaypalExpressCartLoader _paypalExpressCartLoader;
        private readonly ICartManager _cartManager;
        private readonly ICartBuilder _cartBuilder;
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public GetPaypalShippingOptions(IPaypalExpressCartLoader paypalExpressCartLoader, ICartManager cartManager, ICartBuilder cartBuilder, IShippingMethodUIService shippingMethodUiService)
        {
            _paypalExpressCartLoader = paypalExpressCartLoader;
            _cartManager = cartManager;
            _cartBuilder = cartBuilder;
            _shippingMethodUIService = shippingMethodUiService;
        }

        public List<PaypalShippingOption> Get(PaypalShippingInfo info)
        {
            var cart = _paypalExpressCartLoader.GetCart(info.Token);
            if (cart == null)
                return new List<PaypalShippingOption>();

            _cartManager.SetShippingAddress(info.ToAddress(), cart.UserGuid);

            cart = _cartBuilder.BuildCart(cart.UserGuid);

            var orderedEnumerable = _shippingMethodUIService.GetEnabledMethods().FindAll(method => method.CanBeUsed(cart))
                .OrderBy(method => method.GetShippingTotal(cart));

            List<PaypalShippingOption> paypalShippingOptions = orderedEnumerable.Select(x => new PaypalShippingOption
            {
                Amount = x.GetShippingTotal(cart),
                Default = false,
                InsuranceAmount = 0m,
                DisplayName = x.DisplayName,
                SystemName = x.TypeName,
                Tax = x.GetShippingTax(cart)
            }).ToList();
            if (paypalShippingOptions.Any())
            {
                paypalShippingOptions[0].Default = true;
            }
            return paypalShippingOptions;
        }
    }

    public class PaypalExpressCartLoader : IPaypalExpressCartLoader
    {
        private readonly ISession _session;
        private readonly ICartBuilder _cartBuilder;

        public PaypalExpressCartLoader(ISession session, ICartBuilder cartBuilder)
        {
            _session = session;
            _cartBuilder = cartBuilder;
        }

        public CartModel GetCart(string token)
        {
            var serializedTxCode = JsonConvert.SerializeObject(token);
            var sessionData = _session.QueryOver<SessionData>().Where(data => data.Key == CartManager.CurrentPayPalExpressToken && data.Data == serializedTxCode).SingleOrDefault();
            return sessionData == null
                       ? null
                       : _cartBuilder.BuildCart(sessionData.UserGuid);
        }
    }

    public interface IPaypalExpressCartLoader
    {
        CartModel GetCart(string token);
    }
}