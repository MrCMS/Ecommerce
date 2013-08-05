using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderShippedEmailToCustomer : IOnOrderShipped
    {
        public int Order { get { return 1; } }
        public void OnOrderShipped(Order order)
        {
            
        }
    }
}