using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory
{
    public interface IRemoveProductVariantWarehouseStock
    {
        void Remove(ProductVariant productVariant);
    }

    public class RemoveProductVariantWarehouseStock : IRemoveProductVariantWarehouseStock
    {
        private readonly ISession _session;

        public RemoveProductVariantWarehouseStock(ISession session)
        {
            _session = session;
        }

        public void Remove(ProductVariant productVariant)
        {
            var warehouseStocks = _session.QueryOver<WarehouseStock>().Where(stock => stock.ProductVariant.Id == productVariant.Id).List();

            if (warehouseStocks.Any())
            {
                _session.Transact(session => warehouseStocks.ForEach(session.Delete));
            }
        }
    }
}