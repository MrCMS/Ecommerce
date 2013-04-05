using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountApplicationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountApplicationService _discountApplicationService;
        private readonly IDiscountManager _discountManager;

        public DiscountApplicationController(IDiscountApplicationService discountApplicationService, IDiscountManager discountManager)
        {
            _discountApplicationService = discountApplicationService;
            _discountManager = discountManager;
        }

        [HttpGet]
        public PartialViewResult LoadDiscountApplicationProperties(string applicationID, int discountID)
        {
            Discount discount = _discountManager.Get(discountID);
            string[] name = applicationID.Split('.');
            return PartialView(name[name.Length - 1],
                discount.Application != null && discount.Application.GetType() == Type.GetType(applicationID) ?
                discount.Application : Activator.CreateInstance(Type.GetType(applicationID)));
        }
    }
}