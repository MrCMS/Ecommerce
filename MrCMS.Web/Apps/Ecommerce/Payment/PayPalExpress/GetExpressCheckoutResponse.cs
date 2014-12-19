using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using PayPal.PayPalAPIInterfaceService.Model;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class GetExpressCheckoutResponse : PayPalResponse
    {
        public bool AddressAndMethodSet { get; set; }
    }
    public class DoExpressCheckoutPaymentResponse : PayPalResponse
    {
        public DoExpressCheckoutPaymentResponseDetailsType Details { get; set; }

        public bool RedirectToPayPal { get; set; }

        public void UpdateOrder(Order order)
        {
            order.AuthorisationToken = Details.Token;
            var paymentInfo = Details.PaymentInfo.FirstOrDefault();
            if (paymentInfo != null)
            {
                order.PaymentStatus = paymentInfo.PaymentStatus.GetPaymentStatus();
                if (order.PaymentStatus.Equals(PaymentStatus.Paid))
                {
                    order.ShippingStatus = ShippingStatus.Unshipped;
                    order.PaidDate = CurrentRequestData.Now;
                }
                order.CaptureTransactionId = paymentInfo.TransactionID;
            }
        }
    }
}