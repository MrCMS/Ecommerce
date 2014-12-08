using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountApplicationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountApplicationAdminService _discountApplicationAdminService;

        public DiscountApplicationController(IDiscountApplicationAdminService discountApplicationAdminService)
        {
            _discountApplicationAdminService = discountApplicationAdminService;
        }

        [HttpGet]
        public ViewResult Add(Discount discount)
        {
            ViewData["discount"] = discount;
            ViewData["application-type-options"] = _discountApplicationAdminService.GetTypeOptions();
            return View();
        }

        [HttpPost]
        [ReturnJsonTrueIfAjaxPost]
        public RedirectToRouteResult Add(
            [IoCModelBinder(typeof (AddDiscountApplicationModelBinder))] DiscountApplication application)
        {
            _discountApplicationAdminService.Add(application);
            return RedirectToAction("Edit", "Discount", new {id = application.Discount.Id});
        }

        public PartialViewResult List(Discount discount)
        {
            ViewData["discount"] = discount;
            return PartialView(_discountApplicationAdminService.GetApplications(discount));
        }

        [HttpGet]
        public ViewResult Edit(DiscountApplication application)
        {
            return View(application);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ReturnJsonTrueIfAjaxPost]
        public RedirectToRouteResult Edit_POST(DiscountApplication application)
        {
            _discountApplicationAdminService.Update(application);
            return RedirectToAction("Edit", "Discount", new { id = application.Discount.Id });
        }

        [HttpGet]
        public ActionResult Delete(DiscountApplication discountApplication)
        {
            return View(discountApplication);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ReturnJsonTrueIfAjaxPost]
        public ActionResult Delete_POST(DiscountApplication application)
        {
            _discountApplicationAdminService.Delete(application);
            return RedirectToAction("Edit", "Discount", new {id = application.Discount.Id});
        }
    }
}