using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;

        public CartController(IGetCart getCart)
        {
            _getCart = getCart;
        }

        public ViewResult Show(Cart page)
        {
            return View(page);
        }

        [HttpGet]
        public ViewResult Details(Cart page)
        {
            return View(_getCart.GetCart());
        }
    }
}