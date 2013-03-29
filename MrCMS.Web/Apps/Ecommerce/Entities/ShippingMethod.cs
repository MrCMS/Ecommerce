using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.DbConfiguration.Mapping;
using NHibernate.Mapping;


namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class ShippingMethod : SiteEntity
    {
        public ShippingMethod()
        {
            ShippingCalculations = new List<ShippingCalculation>();
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<ShippingCalculation> ShippingCalculations { get; set; }
    }
}