using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentNotRequiredController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IPaymentNotRequiredUIService _paymentNotRequiredUIService;
        private readonly IUniquePageService _uniquePageService;

        public PaymentNotRequiredController(CartModel cartModel, IPaymentNotRequiredUIService paymentNotRequiredUIService, IUniquePageService uniquePageService)
        {
            _cartModel = cartModel;
            _paymentNotRequiredUIService = paymentNotRequiredUIService;
            _uniquePageService = uniquePageService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView();
        }


        [HttpPost]
        [ForceImmediateLuceneUpdate]
        [ActionName("Form")]
        public ActionResult Form_POST()
        {
            var response = _paymentNotRequiredUIService.TryPlaceOrder();

            if (response.Success)
            {
                return response.RedirectTo;
            }

            TempData["error-details"] = response.FailureDetails;
            return response.RedirectTo;
        }
    }
}