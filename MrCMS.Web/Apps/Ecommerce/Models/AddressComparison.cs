using System;
using System.Collections.Generic;

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
                CountryCode = address.CountryCode;
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
        public string CountryCode { get; private set; }
        public string PostalCode { get; private set; }
        public string PhoneNumber { get; private set; }

        public static readonly StrictKeyEqualityComparer<IAddress, AddressComparison> Comparer =
            new StrictKeyEqualityComparer<IAddress, AddressComparison>(address => new AddressComparison(address));

        public bool Equals(AddressComparison other)
        {
            return base.Equals(other);
        }
    }
}