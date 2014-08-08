using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Entities.Geographic
{
    public class Country : SiteEntity
    {
        public Country()
        {
            Regions = new List<Region>();
            TaxRates = new List<TaxRate>();
        }

        [Required]
        [Remote("IsUniqueCountry", "Country", AdditionalFields="Id")]
        public virtual string Name { get; set; }
        [Required]
        [DisplayName("ISO Code (2 letters)")]
        [StringLength(2)]
        public virtual string ISOTwoLetterCode { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual IList<Region> Regions { get; set; }
        public virtual IList<TaxRate> TaxRates { get; set; }
    }
}