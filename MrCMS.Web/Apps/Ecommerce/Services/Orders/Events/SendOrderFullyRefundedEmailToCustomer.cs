using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderFullyRefundedEmailToCustomer : IOnOrderFullyRefunded
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendOrderFullyRefundedEmailToCustomerMessageTemplate, Order> _messageParser;

        public SendOrderFullyRefundedEmailToCustomer(ISession session,
            IMessageParser<SendOrderFullyRefundedEmailToCustomerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 1; } }
        public void OnOrderFullyRefunded(Order order, OrderRefund refund)
        {
            var queuedMessage = _messageParser.GetMessage(order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}