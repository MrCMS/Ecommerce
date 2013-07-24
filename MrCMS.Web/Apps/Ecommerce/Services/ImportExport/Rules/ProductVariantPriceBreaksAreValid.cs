using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantPriceBreaksAreValid : IProductVariantImportValidationRule
    {
        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            var errors = new List<string>();
            var productVariantPriceBreaks = new List<PriceBreak>();
            foreach (var item in productVariant.PriceBreaks)
            {
                var priceBreak = new PriceBreak() {Price = item.Value, Quantity = item.Key};
                productVariantPriceBreaks.Add(priceBreak);
                if (!IsPriceBreakQuantityValid(item.Key, productVariantPriceBreaks) ||
                    !IsPriceBreakPriceValid(item.Key, item.Value, productVariant.Price, productVariantPriceBreaks))
                {
                    productVariantPriceBreaks.Remove(priceBreak);
                    errors.Add(
                        string.Format(
                            "Price Break with Quantity: {0} and Price:{1} is not valid for this product variant.",
                            item.Key, item.Value));
                }
            }
            return errors;
        }

        private static bool IsPriceBreakQuantityValid(int quantity, IEnumerable<PriceBreak> productVariantPriceBreaks)
        {
            return quantity > 1 && productVariantPriceBreaks.Where(x => x.Quantity != quantity).All(@break => @break.Quantity != quantity);
        }

        private static bool IsPriceBreakPriceValid(int quantity,decimal price, decimal productVariantPrice, IEnumerable<PriceBreak> productVariantPriceBreaks)
        {
            return price < productVariantPrice && price > 0
                   && productVariantPriceBreaks.Where(x => x.Quantity != quantity).Where(@break => @break.Quantity < quantity).All(@break => @break.Price > price)
                   && productVariantPriceBreaks.Where(x => x.Quantity != quantity).Where(x => x.Quantity != quantity).Where(@break => @break.Quantity > quantity).All(@break => @break.Price < price);
        }
    }
}