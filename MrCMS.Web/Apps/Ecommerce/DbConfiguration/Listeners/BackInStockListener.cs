using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using NHibernate.Event;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners
{
    public class BackInStockListener : IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var productVariant = @event.Entity as ProductVariant;
            if (productVariant == null)
                return;

            var propertyNames = @event.Persister.PropertyNames.ToList();
            var indexOf = propertyNames.IndexOf("StockRemaining");
            if (indexOf < 0)
                return;

            var oldValue = @event.OldState[indexOf] as int?;
            if (oldValue.GetValueOrDefault() != 0)
                return;
            var newValue = @event.State[indexOf] as int?;
            if (newValue.GetValueOrDefault() <= 0)
                return;

            if (newValue > oldValue)
            {
                using (var session = @event.Session.SessionFactory.OpenFilteredSession())
                {
                    session.Transact(sess =>
                    {
                        var variant = sess.Get<ProductVariant>(productVariant.Id);
                        var backInStockProductVariant = new BackInStockProductVariant
                        {
                            ProductVariant = variant,
                            CreatedOn = CurrentRequestData.Now,
                            UpdatedOn = CurrentRequestData.Now,
                            Site = sess.Get<Site>(CurrentRequestData.CurrentSite.Id)
                        };
                        sess.Save(backInStockProductVariant);
                    });
                }
            }
        }
    }
}