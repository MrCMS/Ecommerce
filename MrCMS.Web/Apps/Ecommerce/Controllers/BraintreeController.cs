using System.Web.Mvc;
using MrCMS.Services;
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

        public BraintreeController(IBraintreePaymentService braintreePaymentService, IUniquePageService uniquePageService)
        {
            _braintreePaymentService = braintreePaymentService;
            _uniquePageService = uniquePageService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            var token = _braintreePaymentService.GenerateClientToken();
            return PartialView(new BraintreePostModel { ClientToken = token }); //get
        }

        [HttpPost]
        public ActionResult MakePayment(FormCollection collection)
        {
            BraintreeResponse response = _braintreePaymentService.MakePayment(collection);

            if (response.Success)
            {
                return _uniquePageService.RedirectTo<OrderPlaced>(new { id = response.Order.Guid });
            }

            TempData.ErrorMessages().AddRange(response.Errors);
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }


        public JsonResult GetToken()
        {
            return Json(_braintreePaymentService.GenerateClientToken(), JsonRequestBehavior.AllowGet);
        }
    }
}