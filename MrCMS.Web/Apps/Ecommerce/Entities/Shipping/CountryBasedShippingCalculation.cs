using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class CountryBasedShippingCalculation : SiteEntity, IStandardShippingCalculation
    {
        public virtual string Countries { get; set; }

        [DisplayName("Shipping Criteria"), Required]
        public virtual ShippingCriteria ShippingCriteria { get; set; }

        [DisplayName("Lower Bound"), Required]
        public virtual decimal LowerBound { get; set; }

        [DisplayName("Upper Bound")]
        public virtual decimal? UpperBound { get; set; }

        [DisplayName("Amount"), Required]
        public virtual decimal BaseAmount { get; set; }
    }
}