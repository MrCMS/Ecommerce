using System.ComponentModel;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class AddressData : IAddress
    {
        public virtual string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [DisplayName("County")]
        public string StateProvince { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }

        public virtual Address ToAddress(ISession session, User user)
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
                User = user
            };
        }
    }
}