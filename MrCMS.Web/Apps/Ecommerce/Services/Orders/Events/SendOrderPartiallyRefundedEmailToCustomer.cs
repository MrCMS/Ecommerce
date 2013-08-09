using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderPartiallyRefundedEmailToCustomer : IOnOrderPartiallyRefunded
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendOrderPartiallyRefundedEmailToCustomerMessageTemplate, Order> _messageParser;

        public SendOrderPartiallyRefundedEmailToCustomer(ISession session,
            IMessageParser<SendOrderPartiallyRefundedEmailToCustomerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 2; } }
        public void OnOrderPartiallyRefunded(Order order, OrderRefund orderRefund)
        {
            var queuedMessage = _messageParser.GetMessage(order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}