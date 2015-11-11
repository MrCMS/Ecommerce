using System;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Services.Pricing
{
    public class ProductPricingMethod : IProductPricingMethod
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly TaxSettings _taxSettings;

        public ProductPricingMethod(TaxSettings taxSettings, IGetDefaultTaxRate getDefaultTaxRate)
        {
            _taxSettings = taxSettings;
            _getDefaultTaxRate = getDefaultTaxRate;
        }

        public decimal GetUnitPricePreDiscount(CartItemData cartItemData)
        {
            throw new NotImplementedException();
        }

        public decimal GetUnitTax(CartItemData cartItemData)
        {
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
            {
                return GetUnitTax(cartItemData.Item);
            }
            return Math.Round(GetTax(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitTax(ProductVariant productVariant, decimal discountAmount = 0m)
        {
            if (!_taxSettings.TaxesEnabled)
                return decimal.Zero;

            var taxRatePercentage = GetTaxRatePercentage(productVariant);

            var price = GetUnitPrice(productVariant, _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountAmount);
            return Math.Round(
                price *
                (taxRatePercentage / (taxRatePercentage + 100)), 2,
                MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitPrice(CartItemData cartItemData)
        {
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
            {
                var unitPriceDiscountAmount = cartItemData.UnitDiscountAmount;
                return GetUnitPrice(cartItemData.Item, unitPriceDiscountAmount);
            }
            return Math.Round(GetPrice(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitPricePreTax(CartItemData cartItemData)
        {
            return _taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual
                ? GetUnitPricePreTax(cartItemData.Item, cartItemData.UnitDiscountAmount)
                : Math.Round(GetPricePreTax(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }

        private decimal GetUnitPricePreTax(ProductVariant item, decimal discountAmount)
        {
            return GetUnitPrice(item, discountAmount) - GetUnitTax(item, discountAmount);
        }

        public decimal GetPricePreDiscount(CartItemData cartItemData)
        {
            throw new NotImplementedException();
        }

        public decimal GetTax(CartItemData cartItemData)
        {
            // just multiply up if we're calculating individually
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
                return cartItemData.PricedQuantity * GetUnitTax(cartItemData.Item, cartItemData.UnitDiscountAmount);

            //otherwise we start the logic based on the total here
            if (!_taxSettings.TaxesEnabled)
                return decimal.Zero;


            var productVariant = cartItemData.Item;
            var basePrice = productVariant.BasePrice;
            var quantity = cartItemData.PricedQuantity;
            var totalAmount = basePrice * quantity;
            var taxRatePercentage = GetTaxRatePercentage(productVariant);
            var discountAmount = cartItemData.UnitDiscountAmount * quantity;
            return GetTax(totalAmount, taxRatePercentage, discountAmount);
            //var basePrice = productVariant.BasePrice;
            //var totalAmount = basePrice * quantity;

            //var tax = _taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax
            //    ? Math.Round(totalAmount * (taxRatePercentage / (taxRatePercentage + 100)), 2,
            //        MidpointRounding.AwayFromZero)
            //    : Math.Round(taxRatePercentage / 100m * totalAmount, 2, MidpointRounding.AwayFromZero);

            //return tax;
        }

        public decimal GetTax(decimal basePrice, decimal taxRatePercentage, decimal discountAmount = decimal.Zero)
        {
            var price = GetPrice(basePrice, taxRatePercentage,
                _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountAmount);
            return Math.Round(
                price *
                (taxRatePercentage / (taxRatePercentage + 100)), 2,
                MidpointRounding.AwayFromZero);

        }

        public decimal GetPrice(CartItemData cartItemData)
        {
            // just multiply up if we're calculating individually
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
                return cartItemData.PricedQuantity * GetUnitPrice(cartItemData);

            var productVariant = cartItemData.Item;
            var quantity = cartItemData.PricedQuantity;
            var basePrice = productVariant.BasePrice;
            var totalAmount = basePrice * quantity;
            var taxRatePercentage = GetTaxRatePercentage(productVariant);
            var discount = cartItemData.UnitDiscountAmount * quantity;

            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax)
            {
                return GetPrice(totalAmount, taxRatePercentage, discount);
                //var price = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero);
                //return price;
            }

            var pricePreTax = GetPricePreTax(cartItemData);
            var tax = GetTax(cartItemData);
            return pricePreTax + tax;
        }

        private decimal GetPricePreTaxDiscountAmount(CartItemData cartItemData)
        {
            if (_taxSettings.DiscountOnPrices == DiscountOnPrices.IncludingTax)
            {
                return decimal.Zero;
            }
            var price = cartItemData.Item.BasePrice;
            price -= cartItemData.UnitDiscountAmount;
            var discountAmount = cartItemData.Item.BasePrice - price;
            var taxRatePercentage = GetTaxRatePercentage(cartItemData.Item);
            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax)
            {
                var tax = Math.Round(discountAmount * (taxRatePercentage / (taxRatePercentage + 100)), 2,
                    MidpointRounding.AwayFromZero);
                discountAmount -= tax;
            }
            return discountAmount;
        }

        public decimal GetUnitPrice(ProductVariant productVariant, decimal discountAmount = decimal.Zero)
        {
            return GetPrice(productVariant.BasePrice, GetTaxRatePercentage(productVariant), discountAmount);
        }



        public decimal GetPrice(decimal basePrice, decimal taxRatePercentage, decimal discountAmount = 0m)
        {
            if (_taxSettings.TaxesEnabled)
            {
                if (_taxSettings.DiscountOnPrices == DiscountOnPrices.ExcludingTax)
                {
                    discountAmount = Math.Round(discountAmount + taxRatePercentage / 100m * discountAmount, 2,
                        MidpointRounding.AwayFromZero);
                }
                if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.ExcludingTax)
                {
                    return
                        Math.Round(basePrice + taxRatePercentage / 100m * basePrice, 2, MidpointRounding.AwayFromZero) -
                        discountAmount;
                }
            }
            return basePrice - discountAmount;
        }
        private decimal GetPricePreTax(decimal basePrice, decimal taxRatePercentage, decimal discountAmount = 0m)
        {
            if (!_taxSettings.TaxesEnabled)
                return basePrice - discountAmount;

            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax)
            {
                return GetPrice(basePrice, taxRatePercentage, discountAmount) -
                       GetTax(basePrice, taxRatePercentage, discountAmount);
            }

            //var fullPrice = GetPrice(basePrice, taxRatePercentage);

            var pricePreTax = basePrice;

            var standardTaxAmount =  Math.Round(pricePreTax * (taxRatePercentage / 100m), 2, MidpointRounding.AwayFromZero);
            var taxToSubtract = _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount
                ? GetTax(basePrice, taxRatePercentage, 0m)
                : GetTax(basePrice, taxRatePercentage, discountAmount);

            if (_taxSettings.DiscountOnPrices == DiscountOnPrices.ExcludingTax)
                discountAmount += Math.Round(discountAmount * (taxRatePercentage / 100m), 2, MidpointRounding.AwayFromZero);

            return pricePreTax + standardTaxAmount - taxToSubtract - discountAmount;


            //var pricePreTax = basePrice;

            //if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax)
            //{
            //    pricePreTax = Math.Round(basePrice - basePrice * (taxRatePercentage / (taxRatePercentage + 100m)), 2, MidpointRounding.AwayFromZero);
            //}

            //if (_taxSettings.DiscountOnPrices == DiscountOnPrices.IncludingTax && _taxSettings.ApplyCustomerTax == ApplyCustomerTax.AfterDiscount)
            //{
            //    discountAmount -= Math.Round(discountAmount * (taxRatePercentage / (taxRatePercentage + 100m)), 2,
            //        MidpointRounding.AwayFromZero);
            //}
        }

        public decimal GetPrice(PriceBreak priceBreak)
        {
            return GetPrice(priceBreak.Price, GetTaxRatePercentage(priceBreak.ProductVariant));
        }

        public decimal GetPricePreTax(CartItemData cartItemData)
        {
            // just multiply up if we're calculating individually
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
                return cartItemData.PricedQuantity * GetUnitPricePreTax(cartItemData);

            var productVariant = cartItemData.Item;
            var quantity = cartItemData.PricedQuantity;
            var basePrice = productVariant.BasePrice;
            var totalAmount = basePrice * quantity;
            var taxRatePercentage = GetTaxRatePercentage(productVariant);
            var discount = cartItemData.UnitDiscountAmount * quantity;

            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.ExcludingTax)
            {
                return GetPricePreTax(totalAmount, taxRatePercentage, discount);
            }
            return GetPrice(cartItemData) - GetTax(cartItemData);
        }

        public decimal? GetPreviousPrice(ProductVariant productVariant)
        {
            if (productVariant.PreviousPrice.HasValue)
                return GetPrice(productVariant.PreviousPrice.Value, GetTaxRatePercentage(productVariant));
            return null;
        }

        public decimal? GetDisplayPrice(Product product)
        {
            throw new NotImplementedException();
        }

        public decimal? GetDisplayPreviousPrice(Product product)
        {
            throw new NotImplementedException();
        }

        public decimal? GetDisplayPreviousPrice(ProductVariant productVariant)
        {
            throw new NotImplementedException();
        }

        public decimal GetTaxRatePercentage(ProductVariant productVariant)
        {
            if (productVariant != null && productVariant.TaxRate != null)
                return productVariant.TaxRate.Percentage;

            var taxRate = _getDefaultTaxRate.Get();
            if (taxRate != null)
                return taxRate.Percentage;

            return 0m;
        }
    }

    //{
    //    return productVariant.BasePrice;
    //}

    //public decimal GetUnitTax(CartItemData cartItemData)
    //{
    //    return
    //        Math.Round(
    //            cartItemData.UnitPrice*(cartItemData.TaxRatePercentage/(cartItemData.TaxRatePercentage + 100)), 2,

    //            MidpointRounding.AwayFromZero);

    //public decimal GetUnitPricePreDiscount(ProductVariant productVariant)
    //}
    //    return GetUnitPricePreDiscount(cartItemData.Item);
    //{

    //public decimal GetUnitPricePreDiscount(CartItemData cartItemData)
    //}
    //    _getProductVariantTaxRatePercentage = getProductVariantTaxRatePercentage;
    //{

    //public ProductPricingMethod(IGetProductVariantTaxRatePercentage getProductVariantTaxRatePercentage)

    //private readonly IGetProductVariantTaxRatePercentage _getProductVariantTaxRatePercentage;
    //}

    //public decimal GetUnitTax(ProductVariant productVariant)
    //{
    //    var taxRatePercentage = GetTaxRatePercentage(productVariant);
    //    return
    //        Math.Round(
    //            GetUnitPricePreDiscount(productVariant)*
    //            (taxRatePercentage/(taxRatePercentage + 100)), 2,
    //            MidpointRounding.AwayFromZero);
    //}

    //public decimal GetUnitPrice(CartItemData cartItemData)
    //{
    //    return
    //        Math.Round(Math.Max(cartItemData.UnitPricePreDiscount - cartItemData.UnitDiscountAmount, decimal.Zero),
    //            2, MidpointRounding.AwayFromZero);
    //}

    //public decimal GetUnitPricePreTax(CartItemData cartItemData)
    //{
    //    return cartItemData.UnitPrice - cartItemData.UnitTax;
    //}

    //public decimal GetPricePreDiscount(CartItemData cartItemData)
    //{
    //    return cartItemData.UnitPricePreDiscount * cartItemData.Quantity;
    //}

    //public decimal GetTax(CartItemData cartItemData)
    //{
    //    return cartItemData.UnitTax*cartItemData.PricedQuantity;
    //}

    //public decimal GetPrice(CartItemData cartItemData)
    //{
    //    return GetPriceLogic(cartItemData.UnitPrice, cartItemData.PricedQuantity);
    //}

    //private static decimal GetPriceLogic(decimal unitPrice, int pricedQuantity)
    //{
    //    return unitPrice*pricedQuantity;
    //}

    //public decimal GetPrice(ProductVariant productVariant)
    //{
    //    return GetPrice(productVariant.GetCartItemDataFromProductVariant(this));
    //}

    //public decimal GetPrice(decimal basePrice, decimal taxRatePercentage)
    //{
    //    return basePrice;
    //}

    //public decimal GetPrice(PriceBreak priceBreak)
    //{
    //    return priceBreak.Price;
    //}

    //public decimal GetPricePreTax(CartItemData cartItemData)
    //{
    //    return cartItemData.Price - cartItemData.Tax;
    //}

    //public decimal? GetPreviousPrice(ProductVariant productVariant)
    //{
    //    return productVariant.PreviousPrice == null
    //        ? null
    //        : productVariant.PreviousPrice;
    //}

    //public decimal GetDiscountAmount(CartItemData cartItemData)
    //{
    //    return cartItemData.PricePreDiscount - cartItemData.Price;
    //}

    //public decimal? GetDisplayPrice(Product product)
    //{
    //    if (product.Variants == null || !product.Variants.Any())
    //        return null;

    //    return
    //        product.Variants.Select(variant => new { variant, price = GetPrice(variant) })
    //            .OrderBy(x => x.price)
    //            .First()
    //            .price;
    //}

    //public decimal? GetDisplayPreviousPrice(Product product)
    //{
    //    if (product.Variants == null || !product.Variants.Any())
    //        return null;

    //    var previousPrice = product.Variants.Select(variant => new { variant, price = GetPreviousPrice(variant) })
    //        .OrderBy(x => x.price)
    //        .First()
    //        .price;
    //    if (!previousPrice.HasValue || previousPrice <= GetDisplayPrice(product))
    //        return null;
    //    return previousPrice;
    //}

    //public decimal? GetDisplayPreviousPrice(ProductVariant productVariant)
    //{
    //    var previousPrice = GetPreviousPrice(productVariant);
    //    if (previousPrice == null)
    //        return null;
    //    var price = GetPrice(productVariant);
    //    if (previousPrice <= price)
    //        return null;
    //    return previousPrice;
    //}

    ////public bool FinalPriceIsCalculated => false;
    //public decimal GetTaxRatePercentage(ProductVariant productVariant) => _getProductVariantTaxRatePercentage.GetTaxRatePercentage(productVariant);
}