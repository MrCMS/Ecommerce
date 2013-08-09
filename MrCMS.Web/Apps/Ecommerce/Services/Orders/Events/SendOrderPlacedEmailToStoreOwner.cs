using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPlacedEmailToStoreOwner : IOnOrderPlaced
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendOrderPlacedEmailToStoreOwnerMessageTemplate, Order> _messageParser;

        public SendOrderPlacedEmailToStoreOwner(ISession session,
            IMessageParser<SendOrderPlacedEmailToStoreOwnerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 2; } }
        public void OnOrderPlaced(Order order)
        {
            var queuedMessage = _messageParser.GetMessage(order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}