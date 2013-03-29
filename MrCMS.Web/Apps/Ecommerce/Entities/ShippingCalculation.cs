using System;
using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ShippingCalculation : SiteEntity
    {
        public virtual string Name { get; set; }
        [DisplayName("Shipping Criteria")]
        public virtual ShippingCriteria ShippingCriteria { get; set; }
        [DisplayName("Lower Bound")]
        public virtual decimal LowerBound { get; set; }
        [DisplayName("Upper Bound")]
        public virtual decimal? UpperBound { get; set; }
        public virtual decimal Price { get; set; }

        public virtual Country Country { get; set; }
        [DisplayName("Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }
    }

    public enum ShippingCriteria
    {
        [Description("Based on cart weight")]
        ByWeight = 1,
        [Description("Based on cart total")]
        ByCartTotal = 2
    }
}