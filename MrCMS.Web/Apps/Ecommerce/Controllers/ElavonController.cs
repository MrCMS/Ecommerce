using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ElavonController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IElavonPaymentService _elavonPaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;
        private readonly MrCMS.Services.Resources.IStringResourceProvider _stringResoureProvider;
        private readonly ICartGuidResetter cartGuidReseter;
        private readonly IGetCurrentUser getCurrentUser;
        private ElavonSettings _elavonSettings;

        public ElavonController(IElavonPaymentService elavonPaymentService, ElavonSettings elavonSettings,
                                IUniquePageService uniquePageService, CartModel cartModel, 
                                MrCMS.Services.Resources.IStringResourceProvider stringResoureProvider, 
                                ICartGuidResetter cartGuidReseter, IGetCurrentUser getCurrentUser)
        {
            _elavonPaymentService = elavonPaymentService;
            _elavonSettings = elavonSettings;
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
            _stringResoureProvider = stringResoureProvider;
            this.cartGuidReseter = cartGuidReseter;
            this.getCurrentUser = getCurrentUser;
        }
               
        [HttpGet]
        public PartialViewResult Form()
        {
            var viewModel = new ElavonPaymentDetailsModel()
            {
                ServiceUrl = _elavonSettings.ServiceUrl
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        public ViewResult ConfirmPaymentRequest()
        {
            return View();
        }        

        //Creates an HPP request object
        public JsonResult PaymentRequest()
        {
            string requestResult = string.Empty;

            var hppJson = _elavonPaymentService.BuildChargeRequest(out requestResult);

            if(requestResult != string.Empty) //error happened
            {
                return Json("Something went wrong: " + requestResult, JsonRequestBehavior.AllowGet);               
            }
            else
            {
                return Json(hppJson, JsonRequestBehavior.AllowGet);
            }
        }

         // Process HPP Response from Globalpay service
        public ActionResult Notification()
        {
            var testStop = string.Empty;

            // The field containing the JSON response, i.e. hppResponse         
            var responseJson = Request.Form["hppResponse"];   
           
            var result = (ElavonCustomResult)_elavonPaymentService.HandleNotification(responseJson);

            var transaction = result.ElavonResponse;

            if(result.ExceptionDescription.Equals(string.Empty))
            {
                cartGuidReseter.ResetCartGuid(CurrentRequestData.UserGuid);

                //Charge request declined
                if(result.ElavonResultType == ElavonCustomEnumerations.ResultType.ChargeFailure)
                {
                    TempData.ErrorMessages().Add(_stringResoureProvider.GetValue("payment-elavon-payment-failed-charge-request-declined",
                                           string.Format("Card Charge request for a total amount of {0} declined.", _cartModel.TotalToPay)));

                    return _uniquePageService.RedirectTo<PaymentDetails>();
                }
                else if(result.ElavonResultType == ElavonCustomEnumerations.ResultType.TamperedTotalPay)
                {
                    TempData.ErrorMessages().Add(_stringResoureProvider.GetValue("payment-elavon-payment-failed-incorrect-value",
                                           string.Format("No payment can be found for a total amount of {0}.", _cartModel.TotalToPay)));

                    return _uniquePageService.RedirectTo<PaymentDetails>();
                }

                //Card payment successful
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = transaction.Order.Guid });
            }
            else
            {
                return _uniquePageService.RedirectTo<PaymentDetails>();                             
            }
        }


        //3D Secure 2 authentication process progress notification handlers - Start

        //3DS Method Completion
        public void ThreeDSMethodCompletedNotification()
        {            
            var threeDSMethodData = Request.Form["threeDSMethodData"];

            try
            {
                byte[] data = Convert.FromBase64String(threeDSMethodData);
                string methodUrlResponseString = Encoding.UTF8.GetString(data);

                // map to a custom class MethodUrlResponse
                MethodUrlResponse methodUrlResponse = JsonConvert.DeserializeObject<MethodUrlResponse>(methodUrlResponseString);

                //unique Global Payments identifier for the 3D Secure authentication (Server Transaction ID)
                string threeDSServerTransID = methodUrlResponse.ThreeDSServerTransID; // af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5

                // TODO: notify client-side that the Method URL step is complete
            }

            catch (Exception exce)
            {
                // TODO: add your exception handling here
            }
        }

        //ACS Challenge Completion
        public void ACSChallengeCompletedNotification()
        {
            var cres = Request.Form["cres"];

            try
            {
                byte[] data = Convert.FromBase64String(cres);
                string challengeUrlResponseString = Encoding.UTF8.GetString(data);
                // map to a custom class ChallengeUrlResponse which has String variables for each response element
                ChallengeUrlResponse challengeUrlResponse = JsonConvert.DeserializeObject<ChallengeUrlResponse>(challengeUrlResponseString);

                var threeDSServerTransID = challengeUrlResponse.ThreeDSServerTransID; // af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5
                var acsTransId = challengeUrlResponse.AcsTransID; // 13c701a3-5a88-4c45-89e9-ef65e50a8bf9
                var challengeCompletionInd = challengeUrlResponse.ChallengeCompletionInd; // Y
                var messageType = challengeUrlResponse.MessageType; // Cres
                var messageVersion = challengeUrlResponse.MessageVersion; // 2.1.0
                var transStatus = challengeUrlResponse.TransStatus; // Y

                // TODO: notify client-side that the Challenge step is complete and pass any required data
            }

            catch (Exception exce)
            {
                // TODO: add your exception handling here
            }

        }
        //3D Secure 2 authentication process progress notification handlers - End

    }

    public class MethodUrlResponse
    {
        public string ThreeDSServerTransID { get; set; }  //e.g. af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5

    }


    public class ChallengeUrlResponse
    {
        public string ThreeDSServerTransID { get; set; }  //e.g. af65c369-59b9-4f8d-b2f6-7d7d5f5c69d5
        public string AcsTransID { get; set; } // 13c701a3-5a88-4c45-89e9-ef65e50a8bf9
        public string ChallengeCompletionInd { get; set; } //13c701a3-5a88-4c45-89e9-ef65e50a8bf9
        public string MessageType { get; set; } //Cres
        public string MessageVersion { get; set; } // 2.1.0
        public string TransStatus { get; set; } // Y       

    }
}