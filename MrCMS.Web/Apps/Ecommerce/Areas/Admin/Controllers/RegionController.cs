using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class RegionController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public PartialViewResult Add(Country country)
        {
            return PartialView(new Region {Country = country});
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Add_POST(Region region)
        {
            _regionService.Add(region);

            return RedirectToAction("Index", "Country");
        }

        [HttpGet]
        public PartialViewResult Edit(Region region)
        {
            return PartialView(region);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Edit_POST(Region region)
        {
            _regionService.Update(region);
            return RedirectToAction("Index", "Country");
        }

        [HttpGet]
        public PartialViewResult Delete(Region region)
        {
            return PartialView(region);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(Region region)
        {
            _regionService.Delete(region);
            return RedirectToAction("Index", "Country");
        }
    }
}