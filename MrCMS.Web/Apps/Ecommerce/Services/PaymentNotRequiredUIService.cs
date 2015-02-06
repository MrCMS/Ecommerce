using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Helpers.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class PaymentNotRequiredUIService : IPaymentNotRequiredUIService
    {
        private readonly IUniquePageService _uniquePageService;
        private readonly CartModel _cartModel;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IOrderPlacementService _orderPlacementService;

        public PaymentNotRequiredUIService(IUniquePageService uniquePageService, CartModel cartModel,
            IStringResourceProvider stringResourceProvider, IOrderPlacementService orderPlacementService)
        {
            _uniquePageService = uniquePageService;
            _cartModel = cartModel;
            _stringResourceProvider = stringResourceProvider;
            _orderPlacementService = orderPlacementService;
        }

        public ActionResult RedirectToPaymentDetails()
        {
            return _uniquePageService.RedirectTo<PaymentDetails>();
        }

        public PaymentNotRequiredResult TryPlaceOrder()
        {
            if (_cartModel.AnythingToPay())
            {
                return new PaymentNotRequiredResult
                {
                    Success = false,
                    FailureDetails = new FailureDetails
                    {
                        Message =
                            _stringResourceProvider.GetValue("Payment Not Required Unavailable",
                            "The payment type is invalid for this cart, as there is something to pay"),
                    },
                    RedirectTo = _uniquePageService.RedirectTo<PaymentDetails>()
                };
            }

            if (_cartModel.CannotPlaceOrderReasons.Any())
            {
                return new PaymentNotRequiredResult
                {
                    Success = false,
                    FailureDetails =
                        new FailureDetails
                        {
                            Message = string.Join(", ", _cartModel.CannotPlaceOrderReasons)
                        },
                    RedirectTo = _uniquePageService.RedirectTo<PaymentDetails>()
                };
            }

            var placeOrder = _orderPlacementService.PlaceOrder(_cartModel, order =>
            {
                order.PaymentStatus = PaymentStatus.Paid;
            });

            return new PaymentNotRequiredResult
            {
                Success = true,
                RedirectTo = _uniquePageService.RedirectTo<OrderPlaced>(new {id = placeOrder.Guid})
            };
        }
    }
}