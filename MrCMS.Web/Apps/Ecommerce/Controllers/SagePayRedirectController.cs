using System;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class SagePayRedirectController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cart;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly ISagePayService _sagePayService;
        private readonly IUniquePageService _uniquePageService;

        public SagePayRedirectController(CartModel cart, IOrderPlacementService orderPlacementService,
            ISagePayService sagePayService, IUniquePageService uniquePageService)
        {
            _cart = cart;
            _orderPlacementService = orderPlacementService;
            _sagePayService = sagePayService;
            _uniquePageService = uniquePageService;
        }

        public ActionResult Failed(string vendorTxCode)
        {
            return View("Failed");
        }

        [HttpPost]
        public RedirectResult FailedPost()
        {
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [ForceImmediateLuceneUpdate]
        public ActionResult Success(string vendorTxCode)
        {
            var sagePayResponse = _sagePayService.GetResponse(_cart.UserGuid);
            if (_cart.CartGuid.ToString() == vendorTxCode && sagePayResponse != null && sagePayResponse.WasTransactionSuccessful)
            {
                _orderPlacementService.PlaceOrder(_cart, o =>
                                                    {
                                                        o.PaymentStatus = PaymentStatus.Paid;
                                                        o.ShippingStatus = ShippingStatus.Unshipped;
                                                        o.AuthorisationToken = sagePayResponse.BankAuthCode;
                                                    });


                return View((object)vendorTxCode);
            }
            return View("Failed");
        }

        [HttpPost]
        public RedirectResult SuccessPost(Guid id)
        {
            return _uniquePageService.RedirectTo<OrderPlaced>(new { id });
        }
    }
}