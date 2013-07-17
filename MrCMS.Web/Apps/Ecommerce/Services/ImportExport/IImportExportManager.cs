using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportExportManager
    {
        byte[] ExportProductsToExcel();
        Dictionary<string, List<string>> ImportProductsFromExcel(Stream file);
    }
}