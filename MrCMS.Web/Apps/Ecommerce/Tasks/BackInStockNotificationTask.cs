using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.MessageTemplates;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Tasks
{
    public class BackInStockNotificationTask : BackgroundTask
    {
        private readonly Site _site;

        public BackInStockNotificationTask(Site site)
            : base(site)
        {
            _site = site;
        }

        public override void Execute()
        {
            Session.Transact(session =>
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