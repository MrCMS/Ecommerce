using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public class ImportParams
    {
        public string ImporterType { get; set; }

        public string ConnectionString { get; set; }
    }

    public interface INopProductImportAdminService
    {
        List<SelectListItem> GetImporterOptions();

        ImportResult Import(ImportParams importParams);
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
    }
}