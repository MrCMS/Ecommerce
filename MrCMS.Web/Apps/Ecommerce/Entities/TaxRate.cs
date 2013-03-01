using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class TaxRate : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual decimal Percentage { get; set; }

        public virtual decimal Multiplier
        {
            get { return 1 + (Percentage/100m); }
        }
    }
}