using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Script.Serialization;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class Region : SiteEntity
    {
        public Region()
        {
            TaxRates = new List<TaxRate>();
        }

        public virtual string Name { get; set; }

        public virtual Country Country { get; set; }
        public virtual IList<TaxRate> TaxRates { get; set; }
    }
}