using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Paypoint;
using MrCMS.Web.Apps.Ecommerce.Services.SagePay;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PaymentMethodController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly CartModel _cartModel;
        private readonly IOrderService _orderService;
        private readonly IPayPalExpressService _payPalExpressService;
        private readonly ISagePayService _sagePayService;
        private readonly IUniquePageService _uniquePageService;

        public PaymentMethodController(CartModel cartModel, IOrderService orderService,
            IPayPalExpressService payPalExpressService, 
             ISagePayService sagePayService, IUniquePageService uniquePageService)
        {
            _cartModel = cartModel;
            _orderService = orderService;
            _payPalExpressService = payPalExpressService;
            _sagePayService = sagePayService;
            _uniquePageService = uniquePageService;
        }

        private bool CanPlaceOrder(out RedirectResult redirectResult)
        {
            redirectResult = null;
            if (!_cartModel.CanPlaceOrder)
            {
                TempData["error-details"] = new FailureDetails
                                            {
                                                Message =
                                                    "We were unable to process your order with the specified cart. Please check your details and try again"
                                            };
                {
                    redirectResult = _uniquePageService.RedirectTo<PaymentDetails>();
                    return false;
                }
            }
            return true;
        }
    }
}