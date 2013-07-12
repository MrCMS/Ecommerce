using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsService
    {
        void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport);
        void ImportProduct(ProductImportDataTransferObject dataTransferObject);
        void ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product);
        void ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product);
        void ImportProductImages(ProductImportDataTransferObject dataTransferObject, Product product);
        bool ImportImageToGallery(string fileLocation, MediaCategory mediaCategory);
    }
}