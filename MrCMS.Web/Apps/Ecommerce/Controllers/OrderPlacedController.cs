using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class OrderPlacedController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;
        private readonly IProductSearchService _productSearchService;

        public OrderPlacedController(IGetCart getCart, IProductSearchService productSearchService)
        {
            _productSearchService = productSearchService;
            _getCart = getCart;
        }

        public ViewResult Show(OrderPlaced page)
        {
            ViewBag.ShopUrl = _productSearchService.GetSiteProductSearch().LiveUrlSegment;
            ViewBag.BasketUrl = _getCart.GetSiteCart().LiveUrlSegment;
            return View(page);
        }
    }
}