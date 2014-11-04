using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class SetShippingMethodService : ISetShippingMethodService
    {
        private readonly CartModel _cart;
        private readonly IShippingMethodUIService _shippingMethodUIService;
        private readonly ICartManager _cartManager;
        private readonly IUniquePageService _uniquePageService;

        public SetShippingMethodService(CartModel cart, IShippingMethodUIService shippingMethodUIService,
            ICartManager cartManager, IUniquePageService uniquePageService)
        {
            _cart = cart;
            _shippingMethodUIService = shippingMethodUIService;
            _cartManager = cartManager;
            _uniquePageService = uniquePageService;
        }

        public List<SelectListItem> GetShippingMethodOptions()
        {
            var orderedEnumerable = _shippingMethodUIService.GetEnabledMethods().FindAll(method => method.CanBeUsed(Cart))
                .OrderBy(method => method.GetShippingTotal(Cart));
            if(orderedEnumerable.Any())
            {
                return orderedEnumerable
                    .BuildSelectItemList(
                        method =>
                            string.Format("{0} ({1})", method.DisplayName,
                                method.GetShippingTotal(Cart).ToCurrencyFormat()), method => method.TypeName,
                        method => method == _cart.ShippingMethod, "Please select...");
            }
            return new List<SelectListItem>();
        }

        public CartModel Cart { get { return _cart; } }
        public void SetShippingMethod(IShippingMethod shippingMethod)
        {
            _cartManager.SetShippingMethod(shippingMethod);
        }

        public ActionResult RedirectToShippingDetails()
        {
            return _uniquePageService.RedirectTo<SetShippingDetails>();
        }
    }
}