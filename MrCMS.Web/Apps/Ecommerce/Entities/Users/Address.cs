using System;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Users
{
    public class Address : SiteEntity
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
        public virtual Country Country { get; set; }
        [DisplayName("Postal Code")]
        [Required]
        public virtual string PostalCode { get; set; }
        [DisplayName("Phone Number")]
        [Required]
        public virtual string PhoneNumber { get; set; }

        public virtual Guid UserGuid { get; set; }
    }
}