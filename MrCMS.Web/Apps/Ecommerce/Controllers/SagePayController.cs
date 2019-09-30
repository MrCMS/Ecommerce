using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Website.Controllers;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ISagePayService _sagePayService;
        private readonly CartModel _cartModel;

        public SagePayController(ISagePayService sagePayService, CartModel cartModel)
        {
            _sagePayService = sagePayService;
            _cartModel = cartModel;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            ViewData["error-details"] = _sagePayService.GetFailureDetails(_cartModel.UserGuid);
            var transactionRegistrationResponse = _sagePayService.RegisterTransaction(_cartModel);
            return PartialView(transactionRegistrationResponse);
        }
    }
}