using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CheckoutController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;

        public CheckoutController(CartModel cart)
        {
            _cart = cart;
        }

        [HttpGet]
        public ViewResult Summary()
        {
            return View(_cart);
        }

        [HttpGet]
        public ViewResult OrderEmail()
        {
            return View(_cart);
        }
        
        [HttpGet]
        public ViewResult DeliveryDetails()
        {
            return View(_cart);
        }
    }
}