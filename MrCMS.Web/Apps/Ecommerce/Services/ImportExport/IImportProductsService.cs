using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductsService
    {
        void ImportProductsFromDTOs(IEnumerable<ProductImportDataTransferObject> productsToImport);
        Product ImportProduct(ProductImportDataTransferObject dataTransferObject);
        IEnumerable<ProductSpecificationValue> ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product);
        IEnumerable<ProductVariant> ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product);
        IEnumerable<MediaFile> ImportProductImages(ProductImportDataTransferObject dataTransferObject, Product product);
        bool ImportImageToGallery(string fileLocation, MediaCategory mediaCategory);
    }
}