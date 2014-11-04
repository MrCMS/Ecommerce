using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class WarehousedStockUpdateResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}