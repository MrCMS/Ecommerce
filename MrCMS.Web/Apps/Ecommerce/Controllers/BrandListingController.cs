using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class BrandListingController : MrCMSAppUIController<EcommerceApp>
    {
        public const string BrandsKey = "brands";
        private readonly IBrandListingUIService _brandListingUIService;

        public BrandListingController(IBrandListingUIService brandListingUIService)
        {
            _brandListingUIService = brandListingUIService;
        }

        public ViewResult Show(BrandListing page, int p = 1)
        {
            ViewData[BrandsKey] = _brandListingUIService.GetBrands(p, page.PageSize);
            return View(page);
        }
    }
}