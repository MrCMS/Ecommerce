using System.Web.Mvc;
using MrCMS.Events;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.CustomerFeedback.Entities;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class OrderFeedbackRecieved : IOnUpdated<FeedbackRecord>
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly UrlHelper _urlHelper;

        public OrderFeedbackRecieved(INotificationPublisher notificationPublisher, UrlHelper urlHelper)
        {
            _notificationPublisher = notificationPublisher;
            _urlHelper = urlHelper;
        }

        public void Execute(OnUpdatedArgs<FeedbackRecord> args)
        {
            // if user has completed a feedback form add a message to admin
            if (args.Item.IsCompleted)
            {
                var link = _urlHelper.Action("Show", "FeedbackRecord", new { id = args.Item.Id });
                var message = string.Format("Feedback has been recieved for <a href='{0}'>Order {1}</a>", link, args.Item.Order.Id);

                _notificationPublisher.PublishNotification(message);
            }
        }
    }
}