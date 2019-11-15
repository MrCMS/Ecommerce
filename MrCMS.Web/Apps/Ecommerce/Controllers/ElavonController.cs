using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Models;
using MrCMS.Web.Apps.Ecommerce.Payment.Elavon.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ElavonController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IElavonPaymentService _elavonPaymentService;
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly ICartGuidResetter cartGuidReseter;

        public ElavonController(IElavonPaymentService elavonPaymentService, ElavonSettings elavonSettings,
                                IUniquePageService uniquePageService, CartModel cartModel, IStringResourceProvider stringResourceProvider,
                                ICartGuidResetter cartGuidReseter, IGetCurrentUser getCurrentUser)
        {
            _elavonPaymentService = elavonPaymentService;
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
            _stringResourceProvider = stringResourceProvider;
            this.cartGuidReseter = cartGuidReseter;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView(_elavonPaymentService.GetElavonPaymentDetailsModel());
        }

        //Creates an HPP request object
        public JsonResult PaymentRequest()
        {
            return Json(_elavonPaymentService.GetPaymentRequestResult(), JsonRequestBehavior.AllowGet);
        }

        // Process HPP Response from Globalpay/Elavon service
        public ActionResult Notification()
        {
            var responseJson = Request.Unvalidated().Form["hppResponse"];
            var elavonCustomResult =(ElavonCustomResult)_elavonPaymentService.CheckNotificationResult(responseJson, out bool isSuccessNotification);

            //success - redirect order placed
            if (isSuccessNotification)
            {
                var currentOrder = elavonCustomResult.ElavonResponse.Order;

                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = currentOrder.Guid });
            }
            else //failure - redirect to PaymentDetails page
            {
                //Reset transaction id in order to retry as per Elavon rule
                ResetTransactionId();
                TempData.ErrorMessages().Add(elavonCustomResult.ErrorMessageResource);
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }
        }

        private void ResetTransactionId()
        {
            var currentCartGuid = _cartModel.CartGuid;
            var currentCartUserGuid = _cartModel.UserGuid;
            cartGuidReseter.ResetCartGuid(CurrentRequestData.UserGuid);
        }
    }
}