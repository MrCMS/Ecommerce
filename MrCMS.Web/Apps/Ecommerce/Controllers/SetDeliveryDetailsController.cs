using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetDeliveryDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IOrderShippingService _orderShippingService;
        private readonly ICartManager _cartManager;

        public SetDeliveryDetailsController(CartModel cart, IOrderShippingService orderShippingService, ICartManager cartManager)
        {
            _cart = cart;
            _orderShippingService = orderShippingService;
            _cartManager = cartManager;
        }

        public ActionResult Show(SetDeliveryDetails page)
        {
            if (_cart.Empty)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
            ViewData["shipping-calculations"] = _orderShippingService.GetShippingOptions(_cart);
            ViewData["cart"] = _cart;
            return View(page);
        }

        public PartialViewResult DeliveryAddress()
        {
            Address shippingAddress = null;
            var shippingMethod = _cart.ShippingMethod;
            var country = _cart.Country;
            if (shippingMethod != null && country != null)
            {
                shippingAddress = _cart.ShippingAddress ?? new Address { Country = country };
            }
            return PartialView(shippingAddress);
        }

        [HttpPost]
        public void SetShipping(ShippingCalculation shippingCalculation)
        {
            _cartManager.SetShippingInfo(shippingCalculation);
        }

        public ActionResult SetAddress(Address address)
        {
            _cartManager.SetShippingAddress(address);
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }
    }
}