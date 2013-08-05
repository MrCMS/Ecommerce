using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPlacedEmailToCustomer : IOnOrderPlaced
    {
        public int Order { get { return 1; } }
        public void OnOrderPlaced(Order order)
        {
        }
    }
}