using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using System;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountLimitationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountLimitationService _discountLimitationService;
        private readonly IDiscountManager _discountManager;

        public DiscountLimitationController(IDiscountLimitationService discountLimitationService, IDiscountManager discountManager)
        {
            _discountLimitationService = discountLimitationService;
            _discountManager = discountManager;
        }

        [HttpGet]
        public PartialViewResult LoadDiscountLimitationProperties(string limitationID,int discountID)
        {
                Discount discount = _discountManager.Get(discountID);
                string[] name = limitationID.Split('.');
                return PartialView(name[name.Length - 1],
                    discount.Limitation != null && discount.Limitation.GetType() == Type.GetType(limitationID) ?
                    discount.Limitation : Activator.CreateInstance(Type.GetType(limitationID)));
        }
    }
}