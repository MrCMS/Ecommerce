using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class SetShippingDetailsUIService : ISetShippingDetailsUIService
    {
        private readonly CartModel _cart;
        private readonly IUniquePageService _uniquePageService;

        public SetShippingDetailsUIService(CartModel cart, IUniquePageService uniquePageService)
        {
            _cart = cart;
            _uniquePageService = uniquePageService;
        }

        public CartModel Cart
        {
            get { return _cart; }
        }

        public bool UserRequiresRedirect()
        {
            return UserRedirect() != null;
        }

        public ActionResult UserRedirect()
        {
            if (Cart.Empty)
                return _uniquePageService.RedirectTo<Pages.Cart>();
            if (string.IsNullOrWhiteSpace(Cart.OrderEmail))
                return _uniquePageService.RedirectTo<EnterOrderEmail>();
            if (!Cart.RequiresShipping)
                return _uniquePageService.RedirectTo<PaymentDetails>();
            return null;
        }
    }
}