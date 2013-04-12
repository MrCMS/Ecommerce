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
        private readonly IDiscountManager _discountManager;

        public DiscountLimitationController(IDiscountManager discountManager)
        {
            _discountManager = discountManager;
        }

        [HttpGet]
        public ActionResult LoadDiscountLimitationProperties(Discount discount, string limitationType)
        {
            var limitation = _discountManager.GetLimitation(discount, limitationType);
            return limitation != null
                       ? (ActionResult) PartialView(limitation)
                       : new EmptyResult();
        }
    }
}