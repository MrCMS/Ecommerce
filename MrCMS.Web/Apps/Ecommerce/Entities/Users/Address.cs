using System;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Users
{
    public class Address : SiteEntity
    {
        public Address()
        {
            
        }

        [DisplayName("First Name")]
        public virtual string FirstName { get; set; }
        [DisplayName("Last Name")]
        public virtual string LastName { get; set; }

        public virtual string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }
        public virtual string Title { get; set; }
        public virtual string Company { get; set; }
        [DisplayName("First Address")]
        public virtual string Address1 { get; set; }
        [DisplayName("Second Address")]
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        [DisplayName("State Province")]
        public virtual string StateProvince { get; set; }
        public virtual Country Country { get; set; }
        [DisplayName("Postal Code")]
        public virtual string PostalCode { get; set; }
        [DisplayName("Phone Number")]
        public virtual string PhoneNumber { get; set; }

        public virtual Guid UserGuid { get; set; }
    }
}