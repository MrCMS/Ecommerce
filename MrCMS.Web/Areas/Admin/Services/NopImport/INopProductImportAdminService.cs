using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface INopProductImportAdminService
    {
        List<ImporterDetails> GetImporterOptions();
    }
}