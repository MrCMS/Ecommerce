using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public interface IImportProductVariants
    {
        IEnumerable<ProductVariant> CreateProductVariants(NopImportContext nopImportContext, HashSet<ProductVariantData> productVariants, HashSet<ProductOptionValueData> optionValues, Product product);
    }
}