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

        public void Execute(OrderPartiallyRefundedArgs args)
        {
            var queuedMessage = _messageParser.GetMessage(args.Order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}