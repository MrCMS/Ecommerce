using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services;
using MrCMS.Website.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class StripeController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IStripePaymentService _StripePaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;
        private PaymentIntent _paymentIntent;

        public StripeController(IStripePaymentService StripePaymentService, 
                                IUniquePageService uniquePageService, CartModel cartModel)
        {
            _StripePaymentService = StripePaymentService;
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            //Create Payment Intent using Stripe Payment Service
            _paymentIntent = _StripePaymentService.CreatePaymentIntent(_cartModel.TotalToPay);

            ViewData["ClientSecret"] = _paymentIntent.ClientSecret;
            ViewData["PaymentIntentId"] = _paymentIntent.Id;

            return PartialView(new StripePaymentDetailsModel
            {
                TotalAmount = _cartModel.TotalToPay
            });
        }

        public ActionResult Notification()
        {
            var stripeCustomResult = (StripeCustomResult)_StripePaymentService.HandleNotification(Request);

            if(stripeCustomResult.StripeResultType == StripeCustomEnumerations.ResultType.ChargeSuccess)
            {
                return new System.Web.Mvc.EmptyResult();
            }
            else
            {
                return new System.Web.Mvc.EmptyResult();
            }
        }

        private ActionResult EmptyResult()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult ConfirmPaymentStatus(StripePaymentDetailsModel model)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_HSfKyVKUpA7tADbscgmX9d0w00scE9qsh1";

            if (model.HandleCardPaymentStatus.ToLowerInvariant().Equals("succeeded"))
            {
                //get the order from webhook and update cart status
                var chargesList = _StripePaymentService.GetChargeAttemptesList(model.PaymentIntentId);

                var chargeResult = chargesList.ToList()
                                              .Where(c => c.Status.Equals("succeeded"))
                                              .FirstOrDefault();
                //StripeResponse
                if(chargeResult != null)
                {
                    Payment.Stripe.Models.StripeResponse stripeResponse = _StripePaymentService.BuildMrCMSOrder(chargeResult);

                    return _uniquePageService.RedirectTo<OrderPlaced>(new { id = stripeResponse.Order.Guid });
                }
                else
                {
                    return _uniquePageService.RedirectTo<PaymentDetails>();
                }
            }
            else
            {
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }
        }


        private Payment.Stripe.Models.StripeResponse BuildStripeResponse(Charge charge)
        {
            var testStop = string.Empty;            
            Random rand = new Random();
            int randomOrderId = rand.Next();

            //Partially map Stripe order object to MrCMS order object
            var newOrder = new Entities.Orders.Order();
                newOrder.Id = randomOrderId;
                newOrder.Total = charge.Amount;
                newOrder.Subtotal = charge.Amount;
                newOrder.TotalPaid = charge.Amount;

            var stripeResponse = new Payment.Stripe.Models.StripeResponse()
            {
                Order = newOrder,
                Errors = new System.Collections.Generic.List<string>(),
                Success = true
            };

            return stripeResponse;
        }                  
    }
}