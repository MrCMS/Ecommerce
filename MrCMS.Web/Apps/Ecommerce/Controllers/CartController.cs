using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;
        private readonly ICartManager _cartManager;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;

        public CartController(IGetCart getCart, ICartManager cartManager, IProductService productService, IProductVariantService productVariantService)
        {
            _getCart = getCart;
            _cartManager = cartManager;
            _productService = productService;
            _productVariantService = productVariantService;
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
        [HttpPost]
        public ViewResult EnterOrderEmail(string email)
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
            ViewBag.BasketUrl = UniquePageHelper.GetUrl<EnterOrderEmail>();
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult AddToCart(int Id=0, int IdVariant=0,int quantity=0)
        {
            Product product = _productService.Get(Id);
            ProductVariant productVariant = _productVariantService.Get(IdVariant);
            if (product != null && quantity>0)
                _cartManager.AddToCart(product, quantity);
            if (productVariant != null && quantity > 0)
                _cartManager.AddToCart(productVariant, quantity);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
        }
    }
}