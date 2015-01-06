using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGetLowStockQuery
    {
        IQueryable<ProductVariant> Get(LowStockReportSearchModel searchModel);
    }
}