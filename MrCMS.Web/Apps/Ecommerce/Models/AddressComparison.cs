using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public struct AddressComparison : IEquatable<AddressComparison>, IAddress
    {
        public AddressComparison(IAddress address)
            : this()
        {
            if (address != null)
            {
                FirstName = address.FirstName;
                LastName = address.LastName;
                Title = address.Title;
                Company = address.Company;
                Address1 = address.Address1;
                Address2 = address.Address2;
                City = address.City;
                StateProvince = address.StateProvince;
                _country = address.Country;
                PostalCode = address.PostalCode;
                PhoneNumber = address.PhoneNumber;
            }
        }
        public string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Title { get; private set; }
        public string Company { get; private set; }
        public string Address1 { get; private set; }
        public string Address2 { get; private set; }
        public string City { get; private set; }
        public string StateProvince { get; private set; }
        Country IAddress.Country { get { return _country; } }
        public string PostalCode { get; private set; }
        public string PhoneNumber { get; private set; }
        public string GetDescription(bool removeName = false)
        {

            var addressParts = GetAddressParts(removeName);
            return string.Join(", ", addressParts);
        }
        private IEnumerable<string> GetAddressParts(bool removeName)
        {
            if (!string.IsNullOrWhiteSpace(Name) && !removeName)
                yield return Name;
            if (!string.IsNullOrWhiteSpace(Company))
                yield return Company;
            if (!string.IsNullOrWhiteSpace(Address1))
                yield return Address1;
            if (!string.IsNullOrWhiteSpace(Address2))
                yield return Address2;
            if (!string.IsNullOrWhiteSpace(City))
                yield return City;
            if (!string.IsNullOrWhiteSpace(StateProvince))
                yield return StateProvince;
            if (_country != null)
                yield return _country.Name;
            if (!string.IsNullOrWhiteSpace(PostalCode))
                yield return PostalCode;
        }

        public static readonly StrictKeyEqualityComparer<IAddress, AddressComparison> Comparer =
            new StrictKeyEqualityComparer<IAddress, AddressComparison>(address => new AddressComparison(address));

        private readonly Country _country;

        public bool Equals(AddressComparison other)
        {
            return base.Equals(other);
        }
    }
}