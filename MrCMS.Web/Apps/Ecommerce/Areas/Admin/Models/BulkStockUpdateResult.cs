using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class BulkStockUpdateResult
    {
        private BulkStockUpdateResult()
        {
        }

        public bool IsSuccess { get; private set; }
        public List<string> Messages { get; private set; }

        public static BulkStockUpdateResult Success(IEnumerable<string> messages = null)
        {
            return new BulkStockUpdateResult
            {
                IsSuccess = true,
                Messages = (messages ?? new List<string>()).ToList()
            };
        }

        public static BulkStockUpdateResult Failure(IEnumerable<string> messages)
        {
            return new BulkStockUpdateResult
            {
                IsSuccess = false,
                Messages = (messages ?? new List<string>()).ToList()
            };
        }
    }
}