using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Geographic
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