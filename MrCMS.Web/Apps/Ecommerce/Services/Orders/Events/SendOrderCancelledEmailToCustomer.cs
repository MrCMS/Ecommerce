using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderCancelledEmailToCustomer : IOnOrderCancelled
    {
        public int Order { get { return 1; } }
        public void OnOrderCancelled(Order order)
        {
            
        }
    }
}