using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Inventory.BulkStockUpdate.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class GetProductVariantsFromFileResult
    {
        private GetProductVariantsFromFileResult()
        {

        }
        public List<BulkStockUpdateDataTransferObject> DTOs { get; private set; }
        public bool IsSuccess { get; private set; }
        public List<string> Messages { get; private set; }
        public static GetProductVariantsFromFileResult Success(List<BulkStockUpdateDataTransferObject> bulkStockUpdateDataTransferObjects)
        {
            return new GetProductVariantsFromFileResult
            {
                IsSuccess = true,
                DTOs = bulkStockUpdateDataTransferObjects
            };
        }

        public static GetProductVariantsFromFileResult Failure(IEnumerable<string> messages)
        {
            return new GetProductVariantsFromFileResult
            {
                IsSuccess = false,
                Messages = (messages ?? new List<string>()).ToList()
            };
        }
    }
}