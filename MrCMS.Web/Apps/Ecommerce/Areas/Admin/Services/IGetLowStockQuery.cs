using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGetLowStockQuery
    {
        IQueryable<ProductVariant> Get(LowStockReportSearchModel searchModel);
    }

    public class GetLowStockQuery : IGetLowStockQuery
    {
        private readonly EcommerceSettings _ecommerceSettings;

        private readonly ISession _session;

        public GetLowStockQuery(EcommerceSettings ecommerceSettings, ISession session)
        {
            _ecommerceSettings = ecommerceSettings;
            _session = session;
        }

        public IQueryable<ProductVariant> Get(LowStockReportSearchModel searchModel)
        {
            IQueryable<ProductVariant> queryOver =
               _session.Query<ProductVariant>().Where(variant => variant.TrackingPolicy == TrackingPolicy.Track);

            if (_ecommerceSettings.WarehouseStockEnabled)
            {
                queryOver =
                    queryOver.Where(
                        variant => variant.WarehouseStock.Sum(stock => stock.StockLevel) <= searchModel.Threshold);
            }
            else
            {
                queryOver = queryOver.Where(variant => variant.StockRemaining <= searchModel.Threshold);
            }
            return queryOver.Cacheable();
        }
    }
}