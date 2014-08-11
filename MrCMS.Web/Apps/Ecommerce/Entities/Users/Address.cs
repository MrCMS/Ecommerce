using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Users
{
    public class Address : SiteEntity, IAddress, IBelongToUser
    {
        [DisplayName("First Name")]
        [Required]
        public virtual string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public virtual string LastName { get; set; }

        public virtual string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }

        public virtual string Title { get; set; }
        public virtual string Company { get; set; }

        [DisplayName("Address 1")]
        [Required]
        public virtual string Address1 { get; set; }

        [DisplayName("Address 2")]
        public virtual string Address2 { get; set; }

        [Required]
        public virtual string City { get; set; }

        [DisplayName("State/Province")]
        public virtual string StateProvince { get; set; }

        [Required]
        public virtual string CountryCode { get; set; }

        [DisplayName("Postal Code")]
        [Required]
        public virtual string PostalCode { get; set; }

        [DisplayName("Phone Number")]
        [Required]
        public virtual string PhoneNumber { get; set; }

        public virtual User User { get; set; }

        public virtual Address Clone(ISession session)
        {
            return new Address
            {
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                Company = Company,
                CountryCode = CountryCode,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                PostalCode = PostalCode,
                StateProvince = StateProvince,
                Title = Title,
                User = User
            };
        }

        public virtual AddressData ToAddressData(ISession session)
        {
            return new AddressData
            {
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                Company = Company,
                CountryCode = CountryCode,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                PostalCode = PostalCode,
                StateProvince = StateProvince,
                Title = Title,
            };
        }
    }
}