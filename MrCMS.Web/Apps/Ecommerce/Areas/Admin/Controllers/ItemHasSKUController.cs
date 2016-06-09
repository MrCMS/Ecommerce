using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ItemHasSKUController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductService _productService;

        public ItemHasSKUController(IProductService productService)
        {
            _productService = productService;
        }

        public PartialViewResult Fields(ItemHasSKU itemHasSKU)
        {
            return PartialView(itemHasSKU);
        }

        [HttpGet]
        public PartialViewResult SearchSKUs(int page = 1, string query = "")
        {
            var items = _productService.Search(query, page);
            return PartialView(items);
        }
    }
}