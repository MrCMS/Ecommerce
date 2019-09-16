using System;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Stripe.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using Stripe;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class StripeController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IStripePaymentService _StripePaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;

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
            return PartialView(new StripePaymentDetailsModel
            {
                TotalAmount = _cartModel.TotalToPay
            });
        }
        
        [HttpPost]
        public ActionResult RequestCharge(StripePaymentDetailsModel model)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_HSfKyVKUpA7tADbscgmX9d0w00scE9qsh1";

            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = model.SourceToken; // Using ASP.NET MVC

            var options = _StripePaymentService.CreateChargeOptions(token);

            var chargeResult = _StripePaymentService.MakePayment(options);

            if (chargeResult.Success == true)
            {
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = chargeResult.Order.Guid });
            }
            else
            {
                TempData.ErrorMessages().AddRange(chargeResult.Errors);
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }
        }  
        

        private Payment.Stripe.Models.StripeResponse BuildStripeResponse(Charge charge)
        {
            var testStop = string.Empty;

            //map Stripe order object to MrCMS order object
            var newOrder = new Entities.Orders.Order();
            newOrder.Id = Convert.ToInt32(charge.Order.Id);
            newOrder.Total = charge.Order.Amount;
            newOrder.Subtotal = charge.Order.Amount;
            newOrder.TotalPaid = charge.Order.Amount;

            Payment.Stripe.Models.StripeResponse stripeResponse = new Payment.Stripe.Models.StripeResponse()
            {
                Order = newOrder,
                Errors = new System.Collections.Generic.List<string>(),
                Success = true
            };

            return stripeResponse;
        }
    }
}