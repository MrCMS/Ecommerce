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

        public void Execute(OrderPlacedArgs args)
        {
            var queuedMessage = _messageParser.GetMessage(args.Order);
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}