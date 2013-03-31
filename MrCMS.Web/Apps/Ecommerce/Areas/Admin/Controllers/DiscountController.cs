using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountManager _discountService;

        public DiscountController(IDiscountManager discountService)
        {
            _discountService = discountService;
        }

        public ViewResult Index()
        {
            var discounts = _discountService.GetAll();
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
            
            _discountService.Add(discount);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult Edit(Discount discount)
        {
            return PartialView(discount);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(Discount discount, FormCollection formCollection)
        {
            object nesto = Activator.CreateInstance(Type.GetType(formCollection["LimitationOpt"]));
            TryUpdateModel(nesto);
            DiscountLimitation discLimit= (DiscountLimitation)nesto;
            discLimit.DiscountLimitationType = formCollection["LimitationOpt"];
            discLimit.Site = discount.Site;
            discount.Limitation = discLimit;
            _discountService.Save(discount);

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
            _discountService.Delete(discount);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult LoadDiscountLimitationProperties(string lID,int dID)
        {
            Discount discount = _discountService.Get(dID);
            string[] name = lID.Split('.');
            return PartialView("_" + name[name.Length - 1], 
                discount.Limitation != null && discount.Limitation.GetType() == Type.GetType(lID) ? 
                discount.Limitation : Activator.CreateInstance(Type.GetType(lID)));
        }
        [HttpGet]
        public PartialViewResult LoadDiscountApplicationProperties(string aID, int dID)
        {
            Discount discount = _discountService.Get(dID);
            string[] name = aID.Split('.');
            return PartialView("_" + name[name.Length - 1],
                discount.Application != null && discount.Application.GetType() == Type.GetType(aID) ?
                discount.Application : Activator.CreateInstance(Type.GetType(aID)));
        }
    }
}