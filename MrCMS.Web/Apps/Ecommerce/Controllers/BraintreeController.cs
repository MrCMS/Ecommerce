using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Braintree.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class BraintreeController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IBraintreePaymentService _braintreePaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;

        public BraintreeController(IBraintreePaymentService braintreePaymentService, 
            IUniquePageService uniquePageService, CartModel cartModel)
        {
            _braintreePaymentService = braintreePaymentService;
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            ViewData["expiry-months"] = _braintreePaymentService.ExpiryMonths();
            ViewData["expiry-years"] = _braintreePaymentService.ExpiryYears();
            ViewData["token"] = _braintreePaymentService.GenerateClientToken();
            return PartialView(new BraintreePaymentDetailsModel{ PostalCode = _cartModel.BillingAddress.PostalCode, TotalToPay = _cartModel.TotalToPay });
        }

        [HttpPost]
        public ActionResult MakePaymentCard(string nonce, bool? liabilityShifted, bool? liabilityShiftPossible)
        {
            BraintreeResponse response = _braintreePaymentService.MakePayment(nonce);

            if (response.Success)
            {
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = response.Order.Guid });
            }

            TempData.ErrorMessages().AddRange(response.Errors);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [HttpPost]
        public ActionResult MakePaymentPaypal(string nonce)
        {
            BraintreeResponse response = _braintreePaymentService.MakePaymentPaypal(nonce);
            if (response.Success)
                return _uniquePageService.RedirectTo<OrderPlaced>(new {id = response.Order.Guid});

            TempData.ErrorMessages().AddRange(response.Errors);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

    }
}