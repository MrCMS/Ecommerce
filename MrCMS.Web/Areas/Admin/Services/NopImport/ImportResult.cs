using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class ImportResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
    }
}