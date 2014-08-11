using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Shipping
{
    public class UKFirstClassShippingCalculation : SiteEntity, IStandardShippingCalculation
    {
        [DisplayName("Shipping Criteria")]
        [Required]
        public virtual ShippingCriteria ShippingCriteria { get; set; }

        [DisplayName("Lower Bound")]
        [Required]
        [Remote("IsValidShippingCalculation", "UKFirstClassShippingCalculation",
            AdditionalFields = "Id,ShippingCriteria,UpperBound")]
        public virtual decimal LowerBound { get; set; }

        [DisplayName("Upper Bound")]
        public virtual decimal? UpperBound { get; set; }

        [DisplayName("Amount")]
        [Required]
        public virtual decimal BaseAmount { get; set; }
    }
}