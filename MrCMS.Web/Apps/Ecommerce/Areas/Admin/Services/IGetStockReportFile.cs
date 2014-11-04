using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IGetStockReportFile
    {
        byte[] GetFile(IEnumerable<ProductVariant> variants);
    }
}