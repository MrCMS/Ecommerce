using System;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Users
{
    public class Address : SiteEntity
    {
        public Address()
        {
            Orders = new List<Order>();
        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual string Name
        {
            get { return string.Format("{0} {1} {2}", Title, FirstName, LastName); }
        }
        public virtual string Title { get; set; }
        public virtual string Company { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        public virtual string StateProvince { get; set; }
        public virtual Country Country { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string PhoneNumber { get; set; }

        public virtual Guid UserGuid { get; set; }

        public virtual IList<Order> Orders { get; set; }
    }
}