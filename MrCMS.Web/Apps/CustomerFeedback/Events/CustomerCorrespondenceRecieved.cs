using MrCMS.Entities.Notifications;
using MrCMS.Events;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class CustomerCorrespondenceRecieved : IOnAdded<CorrespondenceRecord>
    {
        private readonly INotificationPublisher _notificationPublisher;

        public CustomerCorrespondenceRecieved(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnAddedArgs<CorrespondenceRecord> args)
        {
            if (args.Item.CorrespondenceDirection == CorrespondenceDirection.Incoming)
            {
                var message = string.Format("Customer Correspondence Recieved for Order {0}", args.Item.Order.Id);

                _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
            }
        }
    }
}