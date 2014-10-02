using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class GetWarehouseStockFromFileResult
    {
        private GetWarehouseStockFromFileResult()
        {
        }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }

        public List<BulkWarehouseStockUpdateDTO> DTOs { get; set; }

        public static GetWarehouseStockFromFileResult Failure(List<string> messages)
        {
            return new GetWarehouseStockFromFileResult {Messages = messages};
        }

        public static GetWarehouseStockFromFileResult Success(List<BulkWarehouseStockUpdateDTO> dtos)
        {
            return new GetWarehouseStockFromFileResult
            {
                IsSuccess = true,
                DTOs = dtos
            };
        }
    }
}