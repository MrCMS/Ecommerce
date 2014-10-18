using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class AddressExtensions
    {
        public static string ToJSON(this IAddress address)
        {
            return JsonConvert.SerializeObject(new AddressComparison(address));
        }

        public static string GetDescription(this IAddress address, bool removeName = false)
        {
            IEnumerable<string> addressParts = GetAddressParts(address, removeName);
            return string.Join(", ", addressParts);
        }

        public static string GetDescriptionHtml(this IAddress address, bool removeName = false)
        {
            IEnumerable<string> addressParts = GetAddressParts(address, removeName);
            return string.Join("<br />", addressParts);
        }

        public static string GetCountryName(this IAddress address)
        {
            var country = MrCMSApplication.Get<ICountryService>().GetCountryByCode(address.CountryCode);
            return country != null ? country.Name : string.Empty;
        }

        private static IEnumerable<string> GetAddressParts(IAddress address, bool removeName)
        {
            if (address != null)
            {
                if (!string.IsNullOrWhiteSpace(address.Name) && !removeName)
                    yield return address.Name;
                if (!string.IsNullOrWhiteSpace(address.Company))
                    yield return address.Company;
                if (!string.IsNullOrWhiteSpace(address.Address1))
                    yield return address.Address1;
                if (!string.IsNullOrWhiteSpace(address.Address2))
                    yield return address.Address2;
                if (!string.IsNullOrWhiteSpace(address.City))
                    yield return address.City;
                if (!string.IsNullOrWhiteSpace(address.StateProvince))
                    yield return address.StateProvince;
                var countryName = address.GetCountryName();
                if (!string.IsNullOrWhiteSpace(countryName))
                    yield return countryName;
                if (!string.IsNullOrWhiteSpace(address.PostalCode))
                    yield return address.PostalCode;
                
            }
        }
    }
}