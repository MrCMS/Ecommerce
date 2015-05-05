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
            return PartialView(new BraintreePaymentDetailsModel{ PostalCode = _cartModel.BillingAddress.PostalCode});
        }

        [HttpPost]
        public ActionResult MakePayment(FormCollection collection)//BraintreePaymentDetailsModel model
        {
            BraintreeResponse response = _braintreePaymentService.MakePayment(collection);

            if (response.Success)
            {
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = response.Order.Guid });
            }

            TempData.ErrorMessages().AddRange(response.Errors);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        public ActionResult Braintree3DSecure(BraintreePaymentDetailsModel details, string clientToken)
        {
            ViewData["total-to-pay"] = _cartModel.TotalToPay;
            return PartialView(details);
        }
        
        public JsonResult GetToken()
        {
            return Json(_braintreePaymentService.GenerateClientToken(), JsonRequestBehavior.AllowGet);
        }
    }
}