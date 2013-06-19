using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentDetailsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;
        private readonly IProductSearchService _productSearchService;

        public PaymentDetailsController(IGetCart getCart, IProductSearchService productSearchService)
        {
            _productSearchService = productSearchService;
            _getCart = getCart;
        }

        public ViewResult Show(PaymentDetails page)
        {
            ViewBag.ShopUrl = UniquePageHelper.GetUrl<ProductSearch>();
            ViewBag.BasketUrl = UniquePageHelper.GetUrl<Cart>();
            return View(page);
        }
    }
}