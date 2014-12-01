using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface INopDataImportAdminService
    {
        List<SelectListItem> GetImporterOptions();

        ImportResult Import(ImportParams importParams);
    }
}