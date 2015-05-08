using MrCMS.Entities.Notifications;
using MrCMS.Events;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class InitialOrderFeedbackRecieved : IOnUpdated<FeedbackRecord>
    {
        private readonly INotificationPublisher _notificationPublisher;

        public InitialOrderFeedbackRecieved(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public void Execute(OnUpdatedArgs<FeedbackRecord> args)
        {
            if (args.Item.IsCompleted)
            {
                var message = string.Format("Feedback Recieved on Order {0}", args.Item.Order.Id);
                
                _notificationPublisher.PublishNotification(message, PublishType.Both, NotificationType.AdminOnly);
            }
        }
    }
}