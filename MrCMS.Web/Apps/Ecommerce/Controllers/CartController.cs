using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

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
        public ViewResult Details()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult BasicDetails()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult DeliveryDetails()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult OrderEmail()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult EnterOrderEmail()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult OrderPlaced()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult PaymentDetails()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult SetDeliveryDetails()
        {
            return View(_getCart.GetCart());
        }
    }
}