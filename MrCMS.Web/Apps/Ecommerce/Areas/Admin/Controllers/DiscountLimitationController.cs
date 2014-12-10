using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ActionFilters;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountLimitationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountLimitationAdminService _discountLimitationAdminService;

        public DiscountLimitationController(IDiscountLimitationAdminService discountLimitationAdminService)
        {
            _discountLimitationAdminService = discountLimitationAdminService;
        }

        [HttpGet]
        public ViewResult Add(Discount discount)
        {
            ViewData["discount"] = discount;
            ViewData["limitation-type-options"] = _discountLimitationAdminService.GetTypeOptions();
            return View();
        }

        [HttpPost]
        [ReturnJsonTrueIfAjaxPost]
        public RedirectToRouteResult Add(
            [IoCModelBinder(typeof (AddDiscountLimitationModelBinder))] DiscountLimitation limitation)
        {
            _discountLimitationAdminService.Add(limitation);
            return RedirectToAction("Edit", "Discount", new {id = limitation.Discount.Id});
        }

        public PartialViewResult List(Discount discount)
        {
            ViewData["discount"] = discount;
            return PartialView(_discountLimitationAdminService.GetLimitations(discount));
        }

        [HttpGet]
        public ViewResult Edit(DiscountLimitation limitation)
        {
            return View(limitation);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ReturnJsonTrueIfAjaxPost]
        public RedirectToRouteResult Edit_POST(DiscountLimitation limitation)
        {
            _discountLimitationAdminService.Update(limitation);
            return RedirectToAction("Edit", "Discount", new { id = limitation.Discount.Id });
        }

        [HttpGet]
        public ActionResult Delete(DiscountLimitation discountLimitation)
        {
            return View(discountLimitation);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ReturnJsonTrueIfAjaxPost]
        public ActionResult Delete_POST(DiscountLimitation limitation)
        {
            _discountLimitationAdminService.Delete(limitation);
            return RedirectToAction("Edit", "Discount", new {id = limitation.Discount.Id});
        }
    }
}