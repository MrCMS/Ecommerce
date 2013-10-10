using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.MessageTemplates;
using MrCMS.Web.Apps.Amazon.Services.Orders.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using NHibernate;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class SendAmazonOrderShippedEmailToStoreOwner : IOnAmazonOrderPlaced
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate, Order> _messageParser;

        public SendAmazonOrderShippedEmailToStoreOwner(ISession session,
            IMessageParser<SendAmazonOrderPlacedEmailToStoreOwnerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 102; } }
        public void OnAmazonOrderPlaced(AmazonOrder order)
        {
            var queuedMessage = _messageParser.GetMessage(order.Order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}