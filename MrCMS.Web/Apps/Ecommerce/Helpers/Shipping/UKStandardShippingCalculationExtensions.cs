using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Shipping
{
    public static class UKStandardShippingCalculationExtensions
    {
        public static string FormatRestrictions(this IUKStandardShippingCalculation calculation)
        {
            var parts =
                (calculation.RestrictedTo ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim());

            return string.Join(", ", parts);
        }

        public static string FormatExclusions(this IUKStandardShippingCalculation calculation)
        {
            var parts = (calculation.ExcludedFrom ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());

            return string.Join(", ", parts);
        }

        public static HashSet<string> ExcludedPostcodes(this IUKStandardShippingCalculation calculation)
        {
            var excludedPostcodes = (calculation.ExcludedFrom ?? string.Empty);

            return excludedPostcodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToUpper()).ToHashSet();
        }

        public static bool AnyExcludedPostcodes(this IUKStandardShippingCalculation calculation)
        {
            return calculation.ExcludedPostcodes().Any();
        }

        public static HashSet<string> LimitedPostcodes(this IUKStandardShippingCalculation calculation)
        {
            var limitedPostcodes = (calculation.RestrictedTo ?? string.Empty);

            return
                limitedPostcodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToUpper())
                    .ToHashSet();
        }

        public static bool AnyLimitedPostcodes(this IUKStandardShippingCalculation calculation)
        {
            return calculation.LimitedPostcodes().Any();
        }

        public static HashSet<T> FilterToAvailable<T>(this HashSet<T> collection, CartModel cart)
            where T : IUKStandardShippingCalculation
        {
            if (cart.ShippingAddress == null)
                return collection.ToHashSet();
            var formattedPostcode = cart.ShippingAddress.FormattedPostcode();
            // if the postcode is invalid it's likely not set
            if (formattedPostcode == null)
                return collection.ToHashSet();

            // Remove Excluded Postcodes
            collection.RemoveWhere(calc => calc.ExcludedPostcodes().Any(formattedPostcode.StartsWith));

            // if the postcode is valid check if any starts are filtered
            var matches = collection.Where(calc => calc.LimitedPostcodes().Any(formattedPostcode.StartsWith)).ToHashSet();
   
            // if the postcode matches any specific ones
            return matches.Any()
                ? matches
                : collection.FindAll(calc => !calc.LimitedPostcodes().Any());
        }
    }
}