using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public interface IProductPricingMethod
    {
        decimal GetUnitPricePreDiscount(CartItemData cartItemData);
        decimal GetUnitTax(CartItemData cartItemData);
        decimal GetUnitTax(ProductVariant productVariant, decimal discountAmount = 0m, decimal discountPercentage = 0m);
        decimal GetUnitPrice(CartItemData cartItemData);
        decimal GetUnitPrice(ProductVariant productVariant, decimal discountAmount = 0m, decimal discountPercentage = 0m);
        decimal GetUnitPricePreTax(CartItemData cartItemData);

        decimal GetPricePreDiscount(CartItemData cartItemData);
        decimal GetTax(CartItemData cartItemData);
        decimal GetPrice(CartItemData cartItemData);
        decimal GetPrice(decimal basePrice, decimal taxRatePercentage, decimal discountAmount = 0m, decimal discountPercentage = 0m);
        decimal GetPrice(PriceBreak priceBreak);
        decimal GetPricePreTax(CartItemData cartItemData);

        decimal? GetDisplayPrice(Product product);
        decimal? GetDisplayPreviousPrice(Product product);
        decimal? GetDisplayPreviousPrice(ProductVariant productVariant);

        decimal GetTaxRatePercentage(ProductVariant productVariant);
    }
}