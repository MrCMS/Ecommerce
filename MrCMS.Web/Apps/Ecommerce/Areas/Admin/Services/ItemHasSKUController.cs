using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ItemHasSKUController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ISKUSearchService _skuSearchService;

        public ItemHasSKUController(ISKUSearchService skuSearchService)
        {
            _skuSearchService = skuSearchService;
        }

        public PartialViewResult Fields(ItemHasSKU itemHasSKU)
        {
            return PartialView(itemHasSKU);
        }

        [HttpGet]
        public PartialViewResult SearchSKUs(string query, string skus, int page = 1)
        {
            var items = _skuSearchService.Search(query, skus, page);
            return PartialView(items);
        }
    }
}