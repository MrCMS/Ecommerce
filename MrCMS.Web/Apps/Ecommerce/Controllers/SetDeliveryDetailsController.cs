using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SetDeliveryDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IOrderShippingService _orderShippingService;
        private readonly ICartManager _cartManager;
        private readonly IUniquePageService _uniquePageService;

        public SetDeliveryDetailsController(CartModel cart, IOrderShippingService orderShippingService, ICartManager cartManager, IUniquePageService uniquePageService)
        {
            _cart = cart;
            _orderShippingService = orderShippingService;
            _cartManager = cartManager;
            _uniquePageService = uniquePageService;
        }

        public ActionResult Show(SetDeliveryDetails page)
        {
            if (_cart.Empty)
                return _uniquePageService.RedirectTo<Cart>();
            if (string.IsNullOrWhiteSpace(_cart.OrderEmail))
                return _uniquePageService.RedirectTo<EnterOrderEmail>();
            if (!_cart.RequiresShipping)
                return _uniquePageService.RedirectTo<PaymentDetails>();
            ViewData["shipping-calculations"] = _orderShippingService.GetCheapestShippingOptions(_cart);
            ViewData["cart"] = _cart;
            return View(page);
        }

        public PartialViewResult DeliveryAddress()
        {
            Address shippingAddress = _cart.ShippingAddress ?? new Address();
            ViewData["other-addresses"] = _orderShippingService.ExistingAddressOptions(_cart, shippingAddress);
            return PartialView(shippingAddress);
        }

        public ActionResult SetAddress(Address address)
        {
            _cartManager.SetShippingAddress(address);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }
    }
}