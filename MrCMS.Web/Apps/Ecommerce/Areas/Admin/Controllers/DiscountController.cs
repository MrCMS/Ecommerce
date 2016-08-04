using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountAdminService _discountAdminService;

        public DiscountController(IDiscountAdminService discountAdminService)
        {
            _discountAdminService = discountAdminService;
        }

        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.List)]
        public ViewResult Index(DiscountSearchQuery searchQuery)
        {
            ViewData["results"] = _discountAdminService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Add)]
        public PartialViewResult Add()
        {
            return PartialView(new Discount());
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Add)]
        public RedirectToRouteResult Add_POST(Discount discount)
        {
            _discountAdminService.Add(discount);
            return RedirectToAction("Edit", new {id = discount.Id});
        }

        [HttpGet]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Edit)]
        public ViewResult Edit(Discount discount)
        {
            ViewData["usages"] = _discountAdminService.GetUsages(discount);
            return View(discount);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Edit)]
        public RedirectToRouteResult Edit_POST(Discount discount)
        {
            _discountAdminService.Update(discount);

            return RedirectToAction("Edit", new {id = discount.Id});
        }

        [HttpGet]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Delete)]
        public PartialViewResult Delete(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof (DiscountACL), DiscountACL.Delete)]
        public RedirectToRouteResult Delete_POST(Discount discount)
        {
            _discountAdminService.Delete(discount);

            return RedirectToAction("Index");
        }
    }
}