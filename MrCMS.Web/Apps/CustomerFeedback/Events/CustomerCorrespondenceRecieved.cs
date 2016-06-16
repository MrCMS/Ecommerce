using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.CustomerFeedback.Entities;
using MrCMS.Web.Apps.CustomerFeedback.MessageTemplates;

namespace MrCMS.Web.Apps.CustomerFeedback.Events
{
    public class CustomerCorrespondenceRecieved : IOnAdded<CorrespondenceRecord>
    {
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IMessageParser<CustomerServiceReplyMessageTemplate, CorrespondenceRecord> _messageParser;

        public CustomerCorrespondenceRecieved(INotificationPublisher notificationPublisher, 
            IMessageParser<CustomerServiceReplyMessageTemplate, CorrespondenceRecord> messageParser)
        {
            _notificationPublisher = notificationPublisher;
            _messageParser = messageParser;
        }

        public void Execute(OnAddedArgs<CorrespondenceRecord> args)
        {
            if (args.Item.CorrespondenceDirection != CorrespondenceDirection.Incoming)
                return;

            var emailMessage = _messageParser.GetMessage(args.Item);
            if(emailMessage != null)
                _messageParser.QueueMessage(emailMessage);

            var message = string.Format("Customer Correspondence Recieved for <a href='/Admin/CustomerInteraction/ShowInteraction/{0}'>Order {0}</a>", args.Item.Order.Id);
            _notificationPublisher.PublishNotification(message);
        }
    }
}