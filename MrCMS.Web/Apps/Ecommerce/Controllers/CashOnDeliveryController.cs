using System.Web.Mvc;
using FluentNHibernate.Testing.Values;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.CashOnDelivery;
using MrCMS.Website.Controllers;

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
            var result = _cashOnDeliveryUIService.TryPlaceOrder();
            result.CannotPlaceOrderReasons.ForEach(s => TempData.ErrorMessages().Add(s));
            return result.RedirectResult;
        }
    }
}