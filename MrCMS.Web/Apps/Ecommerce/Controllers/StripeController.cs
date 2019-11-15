using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using System.Web.Mvc;
using static MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models.StripeCustomEnumerations;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class StripeController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IUniquePageService _uniquePageService;

        public StripeController(IStripePaymentService stripePaymentService, IUniquePageService uniquePageService)
        {
            _stripePaymentService = stripePaymentService;
            _uniquePageService = uniquePageService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView(_stripePaymentService.GetPaymentDetailsModel());
        }

        [HttpPost]
        public ActionResult ConfirmPaymentStatus(StripePaymentDetailsModel model)
        {
            var stripeResponse = _stripePaymentService.GetChargeAttemptOutcome(model);

            if (stripeResponse.Success)
            {
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = stripeResponse.Order.Guid });
            }

            TempData.ErrorMessages().Add(stripeResponse.ErrorMessage);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }
    }
}