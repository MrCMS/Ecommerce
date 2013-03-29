using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ShippingCalculation : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual int CriteriaId { get; set; }
        public virtual decimal LowerBound { get; set; }
        public virtual decimal UpperBound { get; set; }
        public virtual decimal Price { get; set; }

        public virtual Country Country { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }
    }
}