using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class UpdateStockAdminService : IUpdateStockAdminService
    {
        private readonly ISession _session;

        public UpdateStockAdminService(ISession session)
        {
            _session = session;
        }

        public void UpdateVariantStock(int id, int stockRemaining)
        {
            var productVariant = _session.Get<ProductVariant>(id);
            productVariant.StockRemaining = stockRemaining;
            _session.Transact(session => session.Save(productVariant));
        }
    }
}