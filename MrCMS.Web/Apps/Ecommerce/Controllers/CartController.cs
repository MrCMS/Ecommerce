using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public CartController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public PartialViewResult Show()
        {
            return PartialView(_getCart.GetCart());
        }
    }
}