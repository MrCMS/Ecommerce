using System;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    //public class B2BPricingMethod : IProductPricingMethod
    //{
    //    private readonly IGetProductVariantTaxRatePercentage _getProductVariantTaxRatePercentage;

    //    public B2BPricingMethod(IGetProductVariantTaxRatePercentage getProductVariantTaxRatePercentage)
    //    {
    //        _getProductVariantTaxRatePercentage = getProductVariantTaxRatePercentage;
    //    }

    //    public decimal GetUnitPricePreDiscount(CartItemData cartItemData)
    //    {
    //        return cartItemData.UnitPricePreTax + cartItemData.UnitTax;
    //    }

    //    public decimal GetUnitPricePreDiscount(ProductVariant productVariant)
    //    {
    //        return UnitPricePreTax(productVariant, 1) + GetUnitTax(productVariant);
    //    }

    //    public decimal GetUnitTax(CartItemData cartItemData)
    //    {
    //        return
    //                //Math.Round(
    //                cartItemData.UnitPricePreTax * (cartItemData.TaxRatePercentage / 100);
    //        //, 2, MidpointRounding.AwayFromZero)
    //        ;
    //    }

    //    public decimal GetUnitTax(ProductVariant productVariant)
    //    {
    //        var unitPricePreTax = UnitPricePreTax(productVariant, 1);
    //        return unitPricePreTax * (GetTaxRatePercentage(productVariant)/ 100);
    //    }

    //    public decimal GetUnitPrice(CartItemData cartItemData)
    //    {
    //        return Math.Max(cartItemData.UnitPricePreDiscount - cartItemData.UnitDiscountAmount, decimal.Zero);
    //    }

    //    public decimal GetUnitPricePreTax(CartItemData cartItemData)
    //    {
    //        //return cartItemData.Item.GetUnitPricePreTax(cartItemData.Quantity);
    //        var productVariant = cartItemData.Item;
    //        var pricedQuantity = cartItemData.PricedQuantity;
    //        return UnitPricePreTax(productVariant, pricedQuantity);
    //    }

    //    public decimal GetPricePreDiscount(CartItemData cartItemData)
    //    {
    //        return cartItemData.PricePreTax + cartItemData.Tax;
    //    }

    //    public decimal GetTax(CartItemData cartItemData)
    //    {
    //        return Math.Round(cartItemData.PricePreTax * (cartItemData.TaxRatePercentage / 100), 2,
    //            MidpointRounding.AwayFromZero);
    //    }

    //    public decimal GetPrice(CartItemData cartItemData)
    //    {
    //        return Math.Max(cartItemData.PricePreDiscount - cartItemData.DiscountAmount, decimal.Zero);
    //    }

    //    public decimal GetPrice(ProductVariant productVariant)
    //    {
    //        return GetPrice(productVariant.GetCartItemDataFromProductVariant(this));
    //    }

    //    public decimal GetPrice(decimal basePrice, decimal taxRatePercentage)
    //    {
    //        return basePrice + basePrice * (taxRatePercentage / 100);
    //    }

    //    public decimal GetPrice(PriceBreak priceBreak)
    //    {
    //        return GetPrice(priceBreak.Price, GetTaxRatePercentage(priceBreak.ProductVariant));
    //    }

    //    public decimal GetPricePreTax(CartItemData cartItemData)
    //    {
    //        return cartItemData.UnitPricePreTax * cartItemData.PricedQuantity;
    //    }

    //    public decimal? GetPreviousPrice(ProductVariant productVariant)
    //    {
    //        var previousPrice = productVariant.PreviousPrice;
    //        return previousPrice == null
    //            ? (decimal?)null
    //            : Math.Round(previousPrice.Value + (previousPrice.Value * (GetTaxRatePercentage(productVariant)/ 100m)), 2,
    //                MidpointRounding.AwayFromZero);
    //    }

    //    public decimal GetDiscountAmount(CartItemData cartItemData)
    //    {
    //        return cartItemData.UnitDiscountAmount * cartItemData.PricedQuantity;
    //    }

    //    public decimal? GetDisplayPrice(Product product)
    //    {
    //        if (product.Variants == null || !product.Variants.Any())
    //            return null;

    //        return
    //            product.Variants.Select(variant => new { variant, price = GetPrice(variant) })
    //                .OrderBy(x => x.price)
    //                .First()
    //                .price;
    //    }

    //    public decimal? GetDisplayPreviousPrice(Product product)
    //    {
    //        if (product.Variants == null || !product.Variants.Any())
    //            return null;

    //        var previousPrice = product.Variants.Select(variant => new { variant, price = GetPreviousPrice(variant) })
    //            .OrderBy(x => x.price)
    //            .First()
    //            .price;
    //        if (!previousPrice.HasValue || previousPrice <= GetDisplayPrice(product))
    //            return null;
    //        return previousPrice;
    //    }

    //    public decimal? GetDisplayPreviousPrice(ProductVariant productVariant)
    //    {
    //        var previousPrice = GetPreviousPrice(productVariant);
    //        if (previousPrice == null)
    //            return null;
    //        var price = GetPrice(productVariant);
    //        if (previousPrice <= price)
    //            return null;
    //        return previousPrice;
    //    }

    //    public bool FinalPriceIsCalculated => true;

    //    public decimal GetTaxRatePercentage(ProductVariant productVariant) => _getProductVariantTaxRatePercentage.GetTaxRatePercentage(productVariant);

    //    private static decimal UnitPricePreTax(ProductVariant productVariant, int pricedQuantity)
    //    {
    //        var priceBreaks = productVariant.PriceBreaks;
    //        var priceBreak = priceBreaks != null
    //            ? priceBreaks.OrderBy(x => x.Price).FirstOrDefault(x => x.Quantity <= pricedQuantity)
    //            : null;
    //        if (priceBreak == null)
    //            return productVariant.BasePrice;
    //        return priceBreak.Price;
    //    }
    //}
}