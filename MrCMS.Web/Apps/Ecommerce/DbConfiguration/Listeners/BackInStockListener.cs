using System.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.BackInStockNotification;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using NHibernate;
using NHibernate.Event;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.DbConfiguration.Listeners
{
    public class BackInStockListener : IOnUpdated<ProductVariant>
    {
        private readonly ISession _session;

        public BackInStockListener(ISession session)
        {
            _session = session;
        }

        public void Execute(OnUpdatedArgs<ProductVariant> args)
        {
            var productVariant = args.Original;
            if (productVariant == null)
                return;


            var oldValue = args.Original.StockRemaining;
            if (oldValue > 0)
                return;

            var newValue = args.Item.StockRemaining;
            if (newValue <= 0)
                return;

            if (newValue > oldValue)
            {
                _session.Transact(sess =>
                {
                    var variant = sess.Get<ProductVariant>(productVariant.Id);
                    var backInStockProductVariant = new BackInStockProductVariant
                    {
                        ProductVariant = variant,
                        CreatedOn = CurrentRequestData.Now,
                        UpdatedOn = CurrentRequestData.Now,
                    };
                    sess.Save(backInStockProductVariant);
                });
            }
        }
    }
}