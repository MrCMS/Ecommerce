using System.Linq;
using MrCMS.Services;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Tasks
{
    public class BackInStockNotificationTask : SchedulableTask
    {
        private readonly ISession _session;
        private readonly IMessageParser<ProductBackInStockMessageTemplate, BackInStockNotificationRequest> _messageParser;

        public BackInStockNotificationTask(ISession session, 
            IMessageParser<ProductBackInStockMessageTemplate, BackInStockNotificationRequest> messageParser)
        {
            _session = session;
            _messageParser = messageParser;
        }

        public override int Priority
        {
            get { throw new System.NotImplementedException(); }
        }

        protected override void OnExecute()
        {
            var backInStockProductVariants = _session.QueryOver<BackInStockProductVariant>().Where(variant => !variant.Processed).List();
            if (!backInStockProductVariants.Any())
                return;

            _session.Transact(session =>
            {
                foreach (var inStockProductVariant in backInStockProductVariants)
                {
                    BackInStockProductVariant variant = inStockProductVariant;
                    var notificationRequests =
                        session.QueryOver<BackInStockNotificationRequest>()
                               .Where(request => !request.IsNotified && request.ProductVariant == variant.ProductVariant)
                               .List();

                    foreach (var notificationRequest in notificationRequests)
                    {
                        var queuedMessage = _messageParser.GetMessage(notificationRequest);
                        _messageParser.QueueMessage(queuedMessage);
                        notificationRequest.IsNotified = true;
                        session.Update(notificationRequest);
                    }

                    inStockProductVariant.Processed = true;
                    session.Update(inStockProductVariant);
                }
            });
        }
    }
}