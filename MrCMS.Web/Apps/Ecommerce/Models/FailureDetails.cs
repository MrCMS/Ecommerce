using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class FailureDetails
    {
        public string ErrorCode { get; set; }
        public IEnumerable<string> Details { get; set; }
        public string Message { get; set; }
    }
}