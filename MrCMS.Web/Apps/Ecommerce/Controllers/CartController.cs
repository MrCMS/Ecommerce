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
        private readonly ICartManager _cartManager;
        private readonly IProductService _productService;

        public CartController(IGetCart getCart, ICartManager cartManager, IProductService productService)
        {
            _getCart = getCart;
            _cartManager = cartManager;
            _productService = productService;
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
        [HttpGet]
        public ViewResult CartPanel()
        {
            ViewBag.BasketUrl = _getCart.GetSiteCart().LiveUrlSegment;
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult AddToCart(int Id=0, int quantity=0)
        {
            Product product = _productService.Get(Id);
            if (product != null && quantity>0)
                _cartManager.AddToCart(product, quantity);
            return Redirect("/"+_getCart.GetSiteCart().LiveUrlSegment);
        }
    }
}