using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Helpers.Shipping
{
    public static class CountryBasedShippingCalculationExtensions
    {
        public static HashSet<CountryBasedShippingCalculation> FilterToAvailable(
            this HashSet<CountryBasedShippingCalculation> collection, CartModel cart)
        {
            if (cart.ShippingAddress == null)
                return collection.ToHashSet();
            string countryCode = cart.ShippingAddress.CountryCode;
            // if the country code is not set return all for now
            if (countryCode == null)
                return collection.ToHashSet();
            // if the code is set we check the calculations
            return
                collection.Where(calc =>
                    CountryCodes(calc).Any(s => countryCode.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
                    .ToHashSet();
        }

        public static IEnumerable<string> CountryCodes(
            this CountryBasedShippingCalculation countryBasedShippingCalculation)
        {
            string countries = countryBasedShippingCalculation.Countries ?? string.Empty;

            return countries.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToUpperInvariant());
        }

        public static string DisplayCountries(
            this CountryBasedShippingCalculation countryBasedShippingCalculation)
        {
            List<string> countryCodes = countryBasedShippingCalculation.CountryCodes().ToList();
            var session = MrCMSApplication.Get<ISession>();

            IList<string> names =
                session.QueryOver<Country>()
                    .Where(country => country.ISOTwoLetterCode.IsIn(countryCodes))
                    .OrderBy(country => country.Name)
                    .Asc.Select(country => country.Name)
                    .Cacheable()
                    .List<string>();

            return string.Join(", ", names);
        }
    }
}