using MrCMS.Website.Controllers;
using Stripe;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class StripeWebhookController : MrCMSAppUIController<EcommerceApp>
    {
        //Signing secret copied from the endpoint config section of the dashboard
        const string secret = "whsec_DdBQwdfwo03wV5VnvzEnxhoO3aNssySe";
        
        public StripeWebhookController()
        {
            var testStop = string.Empty;
        }

        [System.Web.Http.HttpPost]
        public ActionResult Post()
        {
            var json = new StreamReader(System.Web.HttpContext.Current.Request.InputStream).ReadToEnd();

            var stripeEvent = EventUtility.ConstructEvent(json,
                                  HttpContext.Request.Headers["Stripe-Signature"], 
                                  secret);
            try
            {
                switch(stripeEvent.Type)
                {
                    case "charge.succeeded":

                        PaymentIntent succeededIntent = (PaymentIntent)stripeEvent.Data.Object;

                        //_logger.LogInformation("Succeeded: {ID}", intent.Id);

                        // Fulfil the customer's purchase
                        var testStop = stripeEvent.Type;
                        break;
                    case "charge.failed":
                        PaymentIntent failedIntent = (PaymentIntent)stripeEvent.Data.Object;

                        //_logger.LogInformation("Failure: {ID}", intent.Id);

                        // Notify the customer that payment failed
                        var testStopTwo = stripeEvent.Type;
                        break;
                    default:
                        // Handle other event types

                        break;
                }
                return new EmptyResult();

            }
            catch (StripeException e)
            {
                // Invalid Signature
                return new EmptyResult();               
            }
            
        }
    }
}