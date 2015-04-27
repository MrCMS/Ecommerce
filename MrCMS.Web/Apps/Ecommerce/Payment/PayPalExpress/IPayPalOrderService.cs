using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using PayPal.PayPalAPIInterfaceService.Model;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public interface IPayPalOrderService
    {
        List<PaymentDetailsType> GetPaymentDetails(CartModel cart);
        string GetBuyerEmail(CartModel cart);
        BasicAmountType GetMaxAmount(CartModel cart);
        List<PaymentDetailsItemType> GetPaymentDetailsItems(CartModel cart);
        BasicAmountType GetItemTotal(CartModel cart);
    }
}