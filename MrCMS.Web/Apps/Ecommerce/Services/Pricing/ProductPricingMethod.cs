using System;
using System.Linq;
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
            return GetUnitPrice(cartItemData.Item, 0, 0);
        }

        public decimal GetUnitTax(CartItemData cartItemData)
        {
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
            {
                return GetUnitTax(cartItemData.Item, cartItemData.UnitDiscountAmount, cartItemData.DiscountPercentage);
            }
            return Math.Round(GetTax(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitTax(ProductVariant productVariant, decimal discountAmount, decimal discountPercentage)
        {
            if (!_taxSettings.TaxesEnabled)
                return decimal.Zero;

            var taxRatePercentage = GetTaxRatePercentage(productVariant);

            var price = GetUnitPrice(productVariant,
                _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountAmount,
                _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountPercentage
                );

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
                var unitPrice = GetUnitPrice(cartItemData.Item, unitPriceDiscountAmount, cartItemData.DiscountPercentage);

                return unitPrice;
            }
            return Math.Round(GetPrice(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitPricePreTax(CartItemData cartItemData)
        {
            return _taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual
                ? GetUnitPricePreTax(cartItemData.Item, cartItemData.UnitDiscountAmount, cartItemData.DiscountPercentage)
                : Math.Round(GetPricePreTax(cartItemData) / cartItemData.PricedQuantity, 2, MidpointRounding.AwayFromZero);
        }
        

        public decimal GetPricePreDiscount(CartItemData cartItemData)
        {
            // just copying the cart item without discount to get a pre discount price
            return GetPrice(new CartItemData
            {
                Item = cartItemData.Item,
                Pricing = cartItemData.Pricing,
                Quantity = cartItemData.Quantity,
            });
        }

        public decimal GetTax(CartItemData cartItemData)
        {
            // just multiply up if we're calculating individually
            if (_taxSettings.TaxCalculationMethod == TaxCalculationMethod.Individual)
                return cartItemData.PricedQuantity *
                       GetUnitTax(cartItemData.Item, cartItemData.UnitDiscountAmount, cartItemData.DiscountPercentage);

            //otherwise we start the logic based on the total here
            if (!_taxSettings.TaxesEnabled)
                return decimal.Zero;


            var productVariant = cartItemData.Item;
            var basePrice = productVariant.BasePrice;
            var quantity = cartItemData.PricedQuantity;
            var totalAmount = basePrice * quantity;
            var taxRatePercentage = GetTaxRatePercentage(productVariant);
            var discountAmount = cartItemData.UnitDiscountAmount * quantity;
            return GetTax(totalAmount, taxRatePercentage, discountAmount, cartItemData.DiscountPercentage);
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
                return GetPrice(totalAmount, taxRatePercentage, discount, cartItemData.DiscountPercentage);
            }

            var pricePreTax = GetPricePreTax(cartItemData);
            var tax = GetTax(cartItemData);
            return pricePreTax + tax;
        }

        public decimal GetPrice(ProductVariant productVariant)
        {
            return GetPrice(productVariant.BasePrice, GetTaxRatePercentage(productVariant), 0, 0);
        }

        public decimal GetUnitPrice(ProductVariant productVariant, decimal discountAmount, decimal discountPercentage)
        {
            return GetPrice(productVariant.BasePrice, GetTaxRatePercentage(productVariant), discountAmount,
                discountPercentage);
        }

        public decimal GetPrice(decimal basePrice, decimal taxRatePercentage, decimal discountAmount,
            decimal discountPercentage)
        {
            if (_taxSettings.DiscountOnPrices == DiscountOnPrices.ExcludingTax)
                discountAmount = Math.Round(discountAmount + taxRatePercentage / 100m * discountAmount, 2,
                    MidpointRounding.AwayFromZero);

            if (!_taxSettings.TaxesEnabled)
                taxRatePercentage = 0;

            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.ExcludingTax)
                basePrice = Math.Round(basePrice + taxRatePercentage / 100m * basePrice, 2, MidpointRounding.AwayFromZero);

            var result = basePrice - discountAmount;

            if (discountPercentage > 0)
                result -= GetPercentage(result, discountPercentage);

            return result;
        }

        public decimal GetPrice(PriceBreak priceBreak)
        {
            return GetPrice(priceBreak.Price, GetTaxRatePercentage(priceBreak.ProductVariant), 0m, 0m);
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
                return GetPricePreTax(totalAmount, taxRatePercentage, discount, cartItemData.DiscountPercentage);
            }
            return GetPrice(cartItemData) - GetTax(cartItemData);
        }

        private class DisplayPriceInfo
        {
            public DisplayPriceInfo(ProductVariant variant, decimal price)
            {
                Variant = variant;
                Price = price;
            }

            public ProductVariant Variant { get; }
            public decimal Price { get; }
        }

        public decimal? GetDisplayPrice(Product product)
        {
            return GetCheapestVariantInfo(product).Price;
        }

        private DisplayPriceInfo GetCheapestVariantInfo(Product product)
        {
            if (!product.Variants.Any())
                return null;
            return product.Variants.Select(variant => new DisplayPriceInfo(variant, GetPrice(variant.BasePrice, GetTaxRatePercentage(variant), 0m, 0m)))
                .OrderBy(x => x.Price)
                .FirstOrDefault();
        }

        public decimal? GetDisplayPreviousPrice(Product product)
        {
            var cheapestVariantInfo = GetCheapestVariantInfo(product);
            return cheapestVariantInfo == null
                ? null
                : GetDisplayPreviousPrice(cheapestVariantInfo.Variant);
        }

        public decimal? GetDisplayPreviousPrice(ProductVariant productVariant)
        {
            if (productVariant.PreviousPrice.HasValue && productVariant.PreviousPrice > productVariant.BasePrice)
            {
                return GetPrice(productVariant.PreviousPrice.Value, GetTaxRatePercentage(productVariant), 0m, 0m);
            }
            return null;
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

        private static decimal GetPercentage(decimal amount, decimal percentage)
        {
            return Math.Round(amount * (percentage / 100m), 2,
                MidpointRounding.AwayFromZero);
        }

        public decimal GetUnitPricePreTax(ProductVariant item, decimal discountAmount, decimal discountPercentage)
        {
            return GetUnitPrice(item, discountAmount, discountPercentage) -
                   GetUnitTax(item, discountAmount, discountPercentage);
        }

        public decimal GetTax(decimal basePrice, decimal taxRatePercentage, decimal discountAmount,
            decimal discountPercentage)
        {
            var price = GetPrice(basePrice, taxRatePercentage,
                _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountAmount,
                _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount ? decimal.Zero : discountPercentage
                );
            return Math.Round(
                price *
                (taxRatePercentage / (taxRatePercentage + 100)), 2,
                MidpointRounding.AwayFromZero);
        }

        private decimal GetPricePreTax(decimal basePrice, decimal taxRatePercentage, decimal discountAmount,
            decimal discountPercentage)
        {
            if (!_taxSettings.TaxesEnabled)
                return basePrice - discountAmount;

            if (_taxSettings.PriceLoadingMethod == PriceLoadingMethod.IncludingTax)
            {
                return GetPrice(basePrice, taxRatePercentage, discountAmount, discountPercentage) -
                       GetTax(basePrice, taxRatePercentage, discountAmount, discountPercentage);
            }

            var pricePreTax = basePrice - GetPercentage(basePrice, discountPercentage);


            var standardTaxAmount = Math.Round(pricePreTax * (taxRatePercentage / 100m), 2, MidpointRounding.AwayFromZero);
            var taxToSubtract = _taxSettings.ApplyCustomerTax == ApplyCustomerTax.BeforeDiscount
                ? GetTax(basePrice, taxRatePercentage, 0m, 0m)
                : GetTax(basePrice, taxRatePercentage, discountAmount, discountPercentage);

            if (_taxSettings.DiscountOnPrices == DiscountOnPrices.ExcludingTax)
                discountAmount += Math.Round(discountAmount * (taxRatePercentage / 100m), 2, MidpointRounding.AwayFromZero);

            return pricePreTax + standardTaxAmount - taxToSubtract - discountAmount;
        }
    }
}