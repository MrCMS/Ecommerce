using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using PayPal.PayPalAPIInterfaceService.Model;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalCartManager
    {
        void UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details);
    }

    public class PayPalCartManager : IPayPalCartManager
    {
        private readonly ICartManager _cartManager;

        public PayPalCartManager(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        public void UpdateCart(GetExpressCheckoutDetailsResponseDetailsType details)
        {
            _cartManager.SetPaymentMethod(new PayPalExpressCheckoutPaymentMethod().SystemName);
            _cartManager.SetPayPalExpressInfo(details.Token,
                                       details.PayerInfo.PayerID);

            _cartManager.SetBillingAddress(details.BillingAddress.GetAddress());

            var paymentDetails = details.PaymentDetails.FirstOrDefault();
            if (paymentDetails != null)
                _cartManager.SetShippingAddress(paymentDetails.ShipToAddress.GetAddress());
        }
    }
}