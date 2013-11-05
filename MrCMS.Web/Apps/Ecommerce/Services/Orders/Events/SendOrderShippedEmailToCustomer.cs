using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderShippedEmailToCustomer : IOnOrderShipped
    {
        private readonly ISession _session;
        private readonly IMessageParser<SendOrderShippedEmailToCustomerMessageTemplate, Order> _messageParser;

        public SendOrderShippedEmailToCustomer(ISession session,
            IMessageParser<SendOrderShippedEmailToCustomerMessageTemplate, Order> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public int Order { get { return 1; } }
        public void OnOrderShipped(Order order)
        {
            if (order.SalesChannel == EcommerceApp.DefaultSalesChannel) //only send if sold on website. Amazon and thirdparties do not allow email sending
            {
                var queuedMessage = _messageParser.GetMessage(order);
                if (queuedMessage != null)
                    _session.Transact(session => session.Save(queuedMessage));
            }
        }
    }
}