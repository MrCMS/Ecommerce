using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPartiallyRefundedEmailToCustomer : IOnOrderPartiallyRefunded
    {
        public int Order { get { return 1; } }
        public void OnOrderPartiallyRefunded(Order order, OrderRefund refund)
        {
            
        }
    }
}