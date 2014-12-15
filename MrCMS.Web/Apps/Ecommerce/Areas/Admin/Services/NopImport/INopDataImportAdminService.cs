using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public interface INopDataImportAdminService
    {
        List<SelectListItem> GetImporterOptions();

        ImportResult Import(ImportParams importParams);
    }
}