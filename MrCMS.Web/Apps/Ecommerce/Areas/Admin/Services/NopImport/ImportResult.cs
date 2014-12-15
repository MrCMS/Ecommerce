using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public class ImportResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
    }
}