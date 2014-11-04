using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class OrderPlacedNotification : IOnOrderPlaced
    {
        private readonly INotificationPublisher _notificationPublisher;

        public OrderPlacedNotification(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }
        public void Execute(OrderPlacedArgs args)
        {
            if (args.Order != null && args.Order.BillingAddress != null && args.Order.SalesChannel == EcommerceApp.DefaultSalesChannel)
            {
                var message =
                    string.Format("A <a href='/Admin/Apps/Ecommerce/Order/Edit/{0}'>new order</a> has been placed by {1}.",
                        args.Order.Id, args.Order.BillingAddress.Name);
                _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
            }
        }
    }
}