using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models.Payment;
using MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CashOnDeliveryController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICashOnDeliveryUIService _cashOnDeliveryUIService;

        public CashOnDeliveryController(ICashOnDeliveryUIService cashOnDeliveryUIService)
        {
            _cashOnDeliveryUIService = cashOnDeliveryUIService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Form")]
        public RedirectResult Form_POST()
        {
            CashOnDeliveryPlaceOrderResult result = _cashOnDeliveryUIService.TryPlaceOrder();
            result.CannotPlaceOrderReasons.ForEach(s => TempData.ErrorMessages().Add(s));
            return result.RedirectResult;
        }
    }
}