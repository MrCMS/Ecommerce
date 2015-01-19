using MrCMS.Entities.Messaging;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders.Events
{
    public class SendOrderShippedEmailToCustomer : IOnOrderShipped
    {
        private readonly IMessageParser<SendOrderShippedEmailToCustomerMessageTemplate, Order> _messageParser;

        public SendOrderShippedEmailToCustomer(
            IMessageParser<SendOrderShippedEmailToCustomerMessageTemplate, Order> messageParser)
        {
            _messageParser = messageParser;
        }

        public void Execute(OrderShippedArgs args)
        {
            Order order = args.Order;
            if (order != null && order.SalesChannel == EcommerceApp.DefaultSalesChannel)
                //only send if sold on website. Amazon and thirdparties do not allow email sending
            {
                QueuedMessage queuedMessage = _messageParser.GetMessage(order);
                if (queuedMessage != null)
                    _messageParser.QueueMessage(queuedMessage);
            }
        }
    }
}