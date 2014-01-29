using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public interface IImportProductVariantPriceBreaksService
    {
        IEnumerable<PriceBreak> ImportVariantPriceBreaks(ProductVariantImportDataTransferObject dto, ProductVariant productVariant);
    }
}