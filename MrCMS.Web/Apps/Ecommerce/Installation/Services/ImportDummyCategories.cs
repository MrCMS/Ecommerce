using System.IO;
using MrCMS.Services.ImportExport;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class ImportDummyCategories : IImportDummyCategories
    {
        private readonly ImportExportManager _importExportManager;

        public ImportDummyCategories(ImportExportManager importExportManager)
        {
            _importExportManager = importExportManager;
        }

        public void Import()
        {
            var memoryStream = new MemoryStream(EcommerceInstallHelper.GetFileFromUrl(EcommerceInstalInfo.CategoryExcelUrl));
            var output = _importExportManager.ImportDocumentsFromExcel(memoryStream);
        }
    }
}