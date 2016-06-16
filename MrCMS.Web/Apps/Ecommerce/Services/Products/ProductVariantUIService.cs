using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models.StockAvailability;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class ProductVariantUIService : IProductVariantUIService
    {
        private readonly IProductVariantAvailabilityService _productVariantAvailabilityService;
        private readonly IProductPricingMethod _productPricingMethod;

        public ProductVariantUIService(IProductVariantAvailabilityService productVariantAvailabilityService,IProductPricingMethod productPricingMethod)
        {
            _productVariantAvailabilityService = productVariantAvailabilityService;
            _productPricingMethod = productPricingMethod;
        }

        public CanBuyStatus CanBuyAny(ProductVariant productVariant)
        {
            return _productVariantAvailabilityService.CanBuy(productVariant, 1);
        }

        public List<SelectListItem> GetProductVariantOptions(ProductVariant productVariant, bool showName = true, bool showOptionValues = true)
        {
            return productVariant.Product.Variants.BuildSelectItemList(variant => GetSelectOptionName(variant, showName, showOptionValues),
                variant => variant.Id.ToString(),
                variant => variant == productVariant,
                emptyItem: null);
        }
        public virtual string GetSelectOptionName(ProductVariant variant, bool showName = true, bool showOptionValues = true)
        {
            string title = string.Empty;
            if (!string.IsNullOrWhiteSpace(variant.Name) && showName)
                title = variant.Name + " - ";

            if (variant.OptionValues.Any() && showOptionValues)
            {
                title += string.Join(", ", variant.AttributeValuesOrdered.Select(value => value.Value)) + " - ";
            }

            title += _productPricingMethod.GetUnitPrice(variant, 0m, 0m).ToCurrencyFormat();

            return title;
        }
    }
}