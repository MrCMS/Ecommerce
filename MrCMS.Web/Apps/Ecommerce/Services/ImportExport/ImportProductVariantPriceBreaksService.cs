using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportProductVariantPriceBreaksService : IImportProductVariantPriceBreaksService
    {
        public IEnumerable<PriceBreak> ImportVariantPriceBreaks(ProductVariantImportDataTransferObject item, ProductVariant productVariant)
        {
            foreach (var priceBreakItem in item.PriceBreaks)
            {
                var priceBreak = productVariant.PriceBreaks.SingleOrDefault(x => x.Quantity == priceBreakItem.Key);
                if (priceBreak == null)
                {
                    priceBreak = new PriceBreak
                        {
                            Price = priceBreakItem.Value,
                            Quantity = priceBreakItem.Key,
                            Item = productVariant
                        };
                    productVariant.PriceBreaks.Add(priceBreak);
                }
            }
            return productVariant.PriceBreaks;
        }
    }
}