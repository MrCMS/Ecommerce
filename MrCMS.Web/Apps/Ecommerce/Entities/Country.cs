using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class Country : SiteEntity
    {
        public Country()
        {
            Regions = new List<Region>();
        }
         public virtual string Name { get; set; }
         public virtual string ISOTwoLetterCode { get; set; }
         public virtual int DisplayOrder { get; set; }
         public virtual IList<Region> Regions { get; set; }
    }
}