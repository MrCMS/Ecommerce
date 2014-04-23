using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using MrCMS.Website;
using NHibernate;
using Ninject;
using PayPal.OpenIdConnect;

namespace MrCMS.Web.Apps.Ecommerce.Tasks
{
    public class BackInStockNotificationTask : SchedulableTask
    {
        private readonly Site _site;
        private readonly ISession _session;

        public BackInStockNotificationTask(Site site, ISession session)
        {
            _site = site;
            _session = session;
        }

        public override int Priority
        {
            get { throw new System.NotImplementedException(); }
        }
        

        protected override void OnExecute()
        {
            _session.Transact(session =>
            {
                var backInStockProductVariants = session.QueryOver<BackInStockProductVariant>().Where(variant => !variant.Processed).List();
                if (!backInStockProductVariants.Any())
                    return;

                var messageParser = new MessageParser<ProductBackInStockMessageTemplate, ProductVariant>(new MessageTemplateParser(MrCMSApplication.Get<IKernel>()), _site, session);

                foreach (var inStockProductVariant in backInStockProductVariants)
                {
                    BackInStockProductVariant variant = inStockProductVariant;
                    var notificationRequests =
                        session.QueryOver<BackInStockNotificationRequest>()
                               .Where(request => !request.IsNotified && request.ProductVariant == variant.ProductVariant)
                               .List();

                    foreach (var notificationRequest in notificationRequests)
                    {
                        var queuedMessage = messageParser.GetMessage(variant.ProductVariant, toAddress: notificationRequest.Email);
                        messageParser.QueueMessage(queuedMessage);
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