using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPlacedEmailToCustomer : IOnOrderPlaced
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendOrderPlacedEmailToCustomerMessageTemplate, Order> _messageParser;

        public SendOrderPlacedEmailToCustomer(ISession session,
            IMessageParser<SendOrderPlacedEmailToCustomerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 1; } }
        public void OnOrderPlaced(Order order)
        {
            var queuedMessage = _messageParser.GetMessage(order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}