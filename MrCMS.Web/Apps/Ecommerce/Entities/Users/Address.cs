using System;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Users
{
    public class Address : SiteEntity
    {
         public virtual string FirstName { get; set; }
         public virtual string LastName { get; set; }
         public virtual string Title { get; set; }
         public virtual string Company { get; set; }
         public virtual string Address1 { get; set; }
         public virtual string Address2 { get; set; }
         public virtual string City { get; set; }
         public virtual string StateProvince { get; set; }
         public virtual Country Country { get; set; }
         public virtual string PhoneNumber { get; set; }

         public virtual Guid UserGuid { get; set; }
    }
}