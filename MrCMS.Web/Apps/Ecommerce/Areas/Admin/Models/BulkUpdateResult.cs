using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class BulkUpdateResult
    {
        public List<string> Messages { get; set; }

        public bool IsSuccess { get; set; }
    }
}