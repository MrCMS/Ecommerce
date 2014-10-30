using System;
using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Helpers.Pricing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Shipping
{
    public static class StandardShippingCalculationExtensions
    {
        public static string GetDescription(this IStandardShippingCalculation calculation)
        {
            switch (calculation.ShippingCriteria)
            {
                case ShippingCriteria.ByWeight:
                    return string.Format("Cart weight: {0}", GetCartWeightValue(calculation));
                case ShippingCriteria.ByCartTotal:
                    return string.Format("Cart total: {0}", GetCartTotalValue(calculation));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool CanBeUsed(this IStandardShippingCalculation calculation, CartModel cart)
        {
            switch (calculation.ShippingCriteria)
            {
                case ShippingCriteria.ByWeight:
                    return IsValid(calculation, cart.Weight);
                case ShippingCriteria.ByCartTotal:
                    return IsValid(calculation, cart.ShippableCalculationTotal());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsValid(IStandardShippingCalculation calculation, decimal value)
        {
            return value >= calculation.LowerBound &&
                   (!calculation.UpperBound.HasValue || value <= calculation.UpperBound);
        }


        private static string GetCartTotalValue(IStandardShippingCalculation calculation)
        {
            return calculation.UpperBound.HasValue
                ? string.Format("{0} to {1}", calculation.LowerBound.ToString("0.00"),
                    calculation.UpperBound.Value.ToString("0.00"))
                : string.Format("{0} or greater", calculation.LowerBound.ToString("0.00"));
        }

        private static string GetCartWeightValue(IStandardShippingCalculation calculation)
        {
            return calculation.UpperBound.HasValue
                ? string.Format("{0}kg to {1}kg", calculation.LowerBound.ToString("0.00"),
                    calculation.UpperBound.Value.ToString("#.##"))
                : string.Format("{0}kg or greater", calculation.LowerBound.ToString("0.00"));
        }

        [DisplayName("Amount Pre Tax")]
        public static decimal AmountPreTax(this IStandardShippingCalculation calculation, decimal taxRate)
        {
            return calculation.BaseAmount.ShippingPriceExcludingTax(taxRate);
        }

        public static decimal Amount(this IStandardShippingCalculation calculation, decimal taxRate)
        {
            return calculation.BaseAmount.ShippingPriceIncludingTax(taxRate);
        }

        public static decimal Tax(this IStandardShippingCalculation calculation, decimal taxRate)
        {
            return calculation.BaseAmount.ShippingTax(taxRate);
        }
    }
}