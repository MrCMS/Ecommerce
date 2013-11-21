using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class DiscountApplicationController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IDiscountManager _discountManager;

        public DiscountApplicationController(IDiscountManager discountManager)
        {
            _discountManager = discountManager;
        }

        [HttpGet]
        public PartialViewResult LoadDiscountApplicationProperties(Discount discount, string applicationType)
        {
            var application = _discountManager.GetApplication(discount, applicationType);
            return PartialView(application.GetType().Name, application);
        }
    }
}