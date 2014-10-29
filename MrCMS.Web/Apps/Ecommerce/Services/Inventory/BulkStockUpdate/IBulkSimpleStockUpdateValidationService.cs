using System.IO;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate
{
    public interface IBulkSimpleStockUpdateValidationService
    {
        GetProductVariantsFromFileResult ValidateFile(Stream file);
    }
}