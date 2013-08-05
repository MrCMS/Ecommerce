using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderFullyRefundedEmailToCustomer : IOnOrderFullyRefunded
    {
        public int Order { get { return 1; } }
        public void OnOrderFullyRefunded(Order order, OrderRefund refund)
        {

        }
    }
}