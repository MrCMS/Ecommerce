using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPlacedEmailToStoreOwner : IOnOrderPlaced
    {
        public int Order { get { return 2; } }
        public void OnOrderPlaced(Order order)
        {
        }
    }
}