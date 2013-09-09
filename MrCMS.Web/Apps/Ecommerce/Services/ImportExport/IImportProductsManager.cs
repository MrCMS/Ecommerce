using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsManager
    {
        Dictionary<string, List<string>> ImportProductsFromExcel(Stream file);
    }
}