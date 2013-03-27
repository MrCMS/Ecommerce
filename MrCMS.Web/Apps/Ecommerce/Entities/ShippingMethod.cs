using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ShippingMethod : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}