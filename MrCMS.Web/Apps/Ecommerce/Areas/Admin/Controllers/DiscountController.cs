using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System;
using MrCMS.Website.Binders;
using MrCMS.Web.Apps.Ecommerce.Binders;

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
        public RedirectToRouteResult Add_POST(Discount discount)
        {
            
            _discountManager.Add(discount);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(Discount discount, [IoCModelBinder(typeof(AddDiscountLimitationModelBinder))] DiscountLimitation limitation, 
            [IoCModelBinder(typeof(AddDiscountApplicationModelBinder))] DiscountApplication application)
        {
            _discountManager.Save(discount, limitation, application);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Delete(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(Discount discount)
        {
            _discountManager.Delete(discount);

            return RedirectToAction("Index");
        }
    }
}