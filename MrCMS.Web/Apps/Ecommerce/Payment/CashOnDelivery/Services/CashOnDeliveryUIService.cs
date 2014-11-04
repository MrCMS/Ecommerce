using System.Collections.Generic;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.Payment;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Payment.CashOnDelivery.Services
{
    public class CashOnDeliveryUIService : ICashOnDeliveryUIService
    {
        private readonly CartModel _cart;
        private readonly IUniquePageService _uniquePageService;
        private readonly IOrderPlacementService _orderPlacementService;

        public CashOnDeliveryUIService(CartModel cart, IUniquePageService uniquePageService, IOrderPlacementService orderPlacementService)
        {
            _cart = cart;
            _uniquePageService = uniquePageService;
            _orderPlacementService = orderPlacementService;
        }

        public CashOnDeliveryPlaceOrderResult TryPlaceOrder()
        {
            if (!_cart.CanPlaceOrder)
            {
                return new CashOnDeliveryPlaceOrderResult
                       {
                           CannotPlaceOrderReasons = new List<string>(_cart.CannotPlaceOrderReasons),
                           RedirectResult = _uniquePageService.RedirectTo<PaymentDetails>()
                       };
            }
            var order = _orderPlacementService.PlaceOrder(_cart, o => { o.PaymentStatus = PaymentStatus.Pending; });
            return new CashOnDeliveryPlaceOrderResult
                   {
                       RedirectResult = _uniquePageService.RedirectTo<OrderPlaced>(new { id = order.Guid })
                   };
        }
    }
}