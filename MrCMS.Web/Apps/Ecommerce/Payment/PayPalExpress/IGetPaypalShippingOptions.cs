using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IGetPaypalShippingOptions
    {
        List<PaypalShippingOption> Get(PaypalShippingInfo info);
    }

    public class GetPaypalShippingOptions : IGetPaypalShippingOptions
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ICartManager _cartManager;
        private readonly IPaypalExpressCartLoader _paypalExpressCartLoader;
        private readonly IShippingMethodUIService _shippingMethodUIService;

        public GetPaypalShippingOptions(IPaypalExpressCartLoader paypalExpressCartLoader, ICartManager cartManager,
            ICartBuilder cartBuilder, IShippingMethodUIService shippingMethodUiService)
        {
            _paypalExpressCartLoader = paypalExpressCartLoader;
            _cartManager = cartManager;
            _cartBuilder = cartBuilder;
            _shippingMethodUIService = shippingMethodUiService;
        }

        public List<PaypalShippingOption> Get(PaypalShippingInfo info)
        {
            CartModel cart = _paypalExpressCartLoader.GetCart(info.Token);
            if (cart == null)
                return new List<PaypalShippingOption>();

            _cartManager.SetShippingAddress(info.ToAddress(), cart.UserGuid);

            cart = _cartBuilder.BuildCart(cart.UserGuid);

            IOrderedEnumerable<IShippingMethod> orderedEnumerable = _shippingMethodUIService.GetEnabledMethods()
                .FindAll(method => method.CanBeUsed(cart))
                .OrderBy(method => method.GetShippingTotal(cart));

            List<PaypalShippingOption> paypalShippingOptions = orderedEnumerable.Select(x => new PaypalShippingOption
            {
                Amount = x.GetShippingTotal(cart),
                Default = false,
                InsuranceAmount = 0m,
                DisplayName = x.DisplayName,
                Label = string.Format("({0})", x.Name),
                TotalTax = cart.ItemTax
            }).ToList();
            if (paypalShippingOptions.Any())
            {
                paypalShippingOptions[0].Default = true;
            }
            return paypalShippingOptions;
        }
    }
}