using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using Stripe;
using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class StripeController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;
        private readonly MrCMS.Services.Resources.IStringResourceProvider _stringResoureProvider;
        private PaymentIntent _paymentIntent;
        private StripeSettings _stripeSettings;

        public StripeController(IStripePaymentService StripePaymentService, StripeSettings stripeSettings,
                                IUniquePageService uniquePageService, CartModel cartModel, 
                                MrCMS.Services.Resources.IStringResourceProvider stringResoureProvider)
        {
            _stripePaymentService = StripePaymentService;
            _stripeSettings = stripeSettings;
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
            _stringResoureProvider = stringResoureProvider;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            //Create Payment Intent using Stripe Payment Service
            _paymentIntent = _stripePaymentService.CreatePaymentIntent(_cartModel.TotalToPay);

            ViewData["ClientSecret"] = _paymentIntent.ClientSecret;
            ViewData["PaymentIntentId"] = _paymentIntent.Id;

            var viewModel = new StripePaymentDetailsModel
            {
                TotalAmount = _cartModel.TotalToPay,
                PublicKey = _stripeSettings.PublicKey,
                CustomerName = _cartModel.User.Name,
                PostalCode = _cartModel.BillingAddress.PostalCode,
                LineOne = _cartModel.BillingAddress.Address1,
                LineTwo = _cartModel.BillingAddress.Address2,
                City = _cartModel.BillingAddress.City,
                Country = _cartModel.BillingAddress.CountryCode,
                State = _cartModel.BillingAddress.StateProvince
            };

            return PartialView(viewModel);
        }

        public ActionResult Notification()
        {
            var stripeCustomResult = (StripeCustomResult)_stripePaymentService.HandleNotification(Request);

            if(stripeCustomResult.StripeResultType == StripeCustomEnumerations.ResultType.ChargeSuccess)
            {
                return new System.Web.Mvc.EmptyResult();
            }
            else
            {
                return new System.Web.Mvc.EmptyResult();
            }
        }

        [HttpPost]
        public ActionResult ConfirmPaymentStatus(StripePaymentDetailsModel model)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = _stripeSettings.PrivateKey;

            if(model.HandleCardPaymentStatus.ToLowerInvariant().Equals("succeeded"))
            {
                //get the order from webhook and update cart status
                var chargesList = _stripePaymentService.GetChargeAttemptesList(model.PaymentIntentId);

                //total pay in pens
                var adjustedTotalPay = (long)(_cartModel.TotalToPay * 100);
                var chargeResult = chargesList.ToList()
                                              .Where(c => c.Status.Equals("succeeded") && 
                                                          c.Amount == adjustedTotalPay)
                                              .FirstOrDefault();
                //StripeResponse
                if(chargeResult != null)
                {
                    Payment.Stripe.Models.StripeResponse stripeResponse = _stripePaymentService.BuildMrCMSOrder(chargeResult);

                    return _uniquePageService.RedirectTo<OrderPlaced>(new { id = stripeResponse.Order.Guid });
                }
                else
                {
                    TempData.ErrorMessages().Add(_stringResoureProvider.GetValue("payment-stripe-payment-failed-incorrect-value", string.Format("No payment can be found for {0}.", _cartModel.TotalToPay)));
                    return _uniquePageService.RedirectTo<PaymentDetails>();
                }
            }
            else
            {
                TempData.ErrorMessages().Add(_stringResoureProvider.GetValue("payment-stripe-payment-failed-errror", $"Your payment was unsuccessful, please try again."));
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }
        }        
        
    }
}