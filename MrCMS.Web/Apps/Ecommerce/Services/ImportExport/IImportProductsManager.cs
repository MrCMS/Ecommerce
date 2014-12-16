using System.Collections.Generic;
using System.IO;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsManager
    {
        ImportProductsResult ImportProductsFromExcel(Stream file, bool autoStartBatch = true);
    }
}