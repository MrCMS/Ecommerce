using System.IO;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class ImportDummyProducts : IImportDummyProducts
    {
        private readonly ImportProductsManager _importExportManager;

        public ImportDummyProducts(ImportProductsManager importExportManager)
        {
            _importExportManager = importExportManager;
        }

        public void Import()
        {
            var memoryStream = new MemoryStream(EcommerceInstallHelper.GetFileFromUrl(EcommerceInstalInfo.ProductsExcelUrl));
            var output = _importExportManager.ImportProductsFromExcel(memoryStream);
        }
    }
}