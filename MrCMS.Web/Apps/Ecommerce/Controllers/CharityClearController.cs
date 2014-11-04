using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.CharityClear.Services;
using MrCMS.Web.Apps.Ecommerce.Payment.WorldPay.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CharityClearController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICharityClearPaymentService _charityClearPaymentService;

        public CharityClearController(ICharityClearPaymentService charityClearPaymentService)
        {
            _charityClearPaymentService = charityClearPaymentService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView(_charityClearPaymentService.GetInfo());
        }

        public ActionResult Notification(FormCollection form)
        {
            var charityClearResponse = _charityClearPaymentService.HandleNotification(form);

            if (charityClearResponse.Success)
                return
                    new RedirectResult(
                        UniquePageHelper.GetAbsoluteUrl<OrderPlaced>(new {id = charityClearResponse.Order.Guid}));
            
            TempData.ErrorMessages().AddRange(charityClearResponse.ErrorMessages);
            return new RedirectResult(UniquePageHelper.GetAbsoluteUrl<PaymentDetails>());
        }
    }
}