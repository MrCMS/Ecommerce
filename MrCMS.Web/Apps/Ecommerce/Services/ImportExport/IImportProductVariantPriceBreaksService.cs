using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductVariantPriceBreaksService
    {
        IImportProductVariantPriceBreaksService Initialize();
        IEnumerable<PriceBreak> ImportVariantPriceBreaks(ProductVariantImportDataTransferObject item, ProductVariant productVariant);
    }
}