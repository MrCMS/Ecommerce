using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class WorldPayController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IWorldPayPaymentService _worldPayPaymentService;

        public WorldPayController(IWorldPayPaymentService worldPayPaymentService)
        {
            _worldPayPaymentService = worldPayPaymentService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            var info = _worldPayPaymentService.GetInfo();
            return PartialView(info);
        }

        public ActionResult Notification()
        {
            return _worldPayPaymentService.HandleNotification(Request);
        }
    }
}