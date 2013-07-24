using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductSpecificationsService
    {
        IEnumerable<ProductSpecificationValue> ImportSpecifications(ProductImportDataTransferObject dataTransferObject, Product product);

        void ImportVariantSpecifications(ProductVariantImportDataTransferObject dataTransferObject, Product product,
                                         ProductVariant productVariant);
    }
}