using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class TriggerOnOrderCancelled : IOnUpdated<Order>
    {
        public void Execute(OnUpdatedArgs<Order> args)
        {
            if (args.Item.IsCancelled && !args.Original.IsCancelled)
            {
                EventContext.Instance.Publish<IOnOrderCancelled, OrderCancelledArgs>(new OrderCancelledArgs
                {
                    Order = args.Item
                });
            }
        }
    }
} 