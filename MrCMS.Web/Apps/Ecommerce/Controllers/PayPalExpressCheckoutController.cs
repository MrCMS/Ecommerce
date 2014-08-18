using System.Text;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class PayPalExpressCheckoutController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IPayPalExpressService _payPalExpressService;
        private readonly CartModel _cart;
        private readonly IOrderPlacementService _orderPlacementService;
        private readonly IPayPalIPNService _payPalIPNService;
        private readonly IUniquePageService _uniquePageService;

        public PayPalExpressCheckoutController(IPayPalExpressService payPalExpressService, CartModel cart,
            IOrderPlacementService orderPlacementService, IPayPalIPNService payPalIPNService, IUniquePageService uniquePageService)
        {
            _payPalExpressService = payPalExpressService;
            _cart = cart;
            _orderPlacementService = orderPlacementService;
            _payPalIPNService = payPalIPNService;
            _uniquePageService = uniquePageService;
        }

        [HttpGet]
        public PartialViewResult Form()
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("Form")]
        [ForceImmediateLuceneUpdate]
        public RedirectResult Form_POST()
        {
            if (!_cart.CanPlaceOrder)
            {
                _cart.CannotPlaceOrderReasons.ForEach(s => TempData.ErrorMessages().Add(s));
                return _uniquePageService.RedirectTo<PaymentDetails>();
            }

            var response = _payPalExpressService.DoExpressCheckout(_cart);

            if (response.Success)
            {
                var order = _orderPlacementService.PlaceOrder(_cart, response.UpdateOrder);
                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>(new { id = order.Guid }));
            }

            else
                TempData["error-details"] = new FailureDetails
                {
                    Message =
                        "An error occurred processing your PayPal Express order, please contact the merchant"
                };
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        [HttpPost]
        public ActionResult SetExpressCheckout()
        {
            var response = _payPalExpressService.GetSetExpressCheckoutRedirectUrl(_cart);

            return response.Success
                       ? Redirect(response.Url)
                       : _uniquePageService.RedirectTo<Cart>();
        }

        public ActionResult Return(string token)
        {
            var response = _payPalExpressService.ProcessReturn(token);

            return response.Success
                       ? _uniquePageService.RedirectTo<SetShippingDetails>()
                       : _uniquePageService.RedirectTo<Cart>();
        }

        public ActionResult IPN()
        {
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string ipnData = Encoding.ASCII.GetString(param);

            _payPalIPNService.HandleIPN(ipnData);

            return Content("");
        }
    }
}