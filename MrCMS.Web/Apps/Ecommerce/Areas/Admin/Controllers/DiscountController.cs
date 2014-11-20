using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Binders;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountManager _discountManager;

        public DiscountController(IDiscountManager discountManager)
        {
            _discountManager = discountManager;
        }

        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.List)]
        public ViewResult Index(DiscountSearchQuery searchQuery)
        {
            ViewData["results"] = _discountManager.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Add)]
        public PartialViewResult Add()
        {
            return PartialView(new Discount());
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Add)]
        public RedirectToRouteResult Add_POST(Discount discount)
        {
            _discountManager.Add(discount);
            return RedirectToAction("Edit", new {id = discount.Id});
        }

        [HttpGet]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Edit)]
        public ViewResult Edit(Discount discount)
        {
            return View(discount);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Edit)]
        public RedirectToRouteResult Edit_POST(Discount discount, [IoCModelBinder(typeof(AddDiscountLimitationModelBinder))] DiscountLimitation limitation, 
            [IoCModelBinder(typeof(AddDiscountApplicationModelBinder))] DiscountApplication application)
        {
            _discountManager.Save(discount, limitation, application);
            
            return RedirectToAction("Edit", new { id = discount.Id });
        }

        [HttpGet]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Delete)]
        public PartialViewResult Delete(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
        [MrCMSACLRule(typeof(DiscountACL), DiscountACL.Delete)]
        public RedirectToRouteResult Delete_POST(Discount discount)
        {
            _discountManager.Delete(discount);

            return RedirectToAction("Index");
        }

        public JsonResult IsUniqueCode(string code, int? id)
        {
            if (_discountManager.IsUniqueCode(code, id))
                return Json(true, JsonRequestBehavior.AllowGet);

            return Json("Code already in use.", JsonRequestBehavior.AllowGet);
        }
    }
}