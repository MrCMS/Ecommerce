using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.DbConfiguration.Mapping;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class Country : SiteEntity
    {
        public Country()
        {
            Regions = new List<Region>();
            TaxRates = new List<TaxRate>();
            ShippingCalculations = new List<ShippingCalculation>();
        }

        public virtual string Name { get; set; }
        public virtual string ISOTwoLetterCode { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual IList<Region> Regions { get; set; }
        public virtual IList<TaxRate> TaxRates { get; set; }
        public virtual IList<ShippingCalculation> ShippingCalculations { get; set; }
    }
}