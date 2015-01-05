using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSearchablePageController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IEcommerceSearchablePageAdminService _ecommerceSearchablePageAdminService;

        public EcommerceSearchablePageController(
            IEcommerceSearchablePageAdminService ecommerceSearchablePageAdminService)
        {
            _ecommerceSearchablePageAdminService = ecommerceSearchablePageAdminService;
        }

        public PartialViewResult HiddenSpecifications(EcommerceSearchablePage searchablePage)
        {
            return PartialView(searchablePage);
        }

        [HttpGet]
        public PartialViewResult AddSpecification(EcommerceSearchablePage searchablePage)
        {
            ViewData["category"] = searchablePage;
            return PartialView(_ecommerceSearchablePageAdminService.GetShownSpecifications(searchablePage));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_ecommerceSearchablePageAdminService.AddSpecificationToHidden(attribute, categoryId));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult RemoveSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_ecommerceSearchablePageAdminService.RemoveSpecificationFromHidden(attribute, categoryId));
        }
    }
}