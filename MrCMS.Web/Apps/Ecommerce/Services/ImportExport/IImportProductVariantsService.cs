using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductVariantsService
    {
        IImportProductVariantsService Initialize();
        IEnumerable<ProductVariant> ImportVariants(ProductImportDataTransferObject dataTransferObject, Product product);
    }
}