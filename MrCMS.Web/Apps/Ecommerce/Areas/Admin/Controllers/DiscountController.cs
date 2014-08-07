using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
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

        public ViewResult Index()
        {
            var discounts = _discountManager.GetAll();
            return View(discounts);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView(new Discount());
        }

        [HttpPost]
        [ActionName("Add")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Add_POST(Discount discount)
        {
            _discountManager.Add(discount);
            return RedirectToAction("Edit", new {id = discount.Id});
        }

        [HttpGet]
        public ViewResult Edit(Discount discount)
        {
            return View(discount);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Edit_POST(Discount discount, [IoCModelBinder(typeof(AddDiscountLimitationModelBinder))] DiscountLimitation limitation, 
            [IoCModelBinder(typeof(AddDiscountApplicationModelBinder))] DiscountApplication application)
        {
            _discountManager.Save(discount, limitation, application);
            
            return RedirectToAction("Edit", new { id = discount.Id });
        }

        [HttpGet]
        public PartialViewResult Delete(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ForceImmediateLuceneUpdate]
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