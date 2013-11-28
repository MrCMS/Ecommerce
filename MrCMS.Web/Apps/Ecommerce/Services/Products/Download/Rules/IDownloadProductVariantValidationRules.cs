using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products.Download.Rules
{
    public interface IDownloadOrderedFileValidationRule
    {
        IEnumerable<string> GetErrors(Order order, OrderLine orderLine);
    }
}