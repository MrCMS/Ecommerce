using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.Shipping
{
    public interface IStandardShippingCalculation
    {
        [DisplayName("Shipping Criteria")]
        [Required]
        ShippingCriteria ShippingCriteria { get; set; }

        [DisplayName("Lower Bound")]
        [Required]
        decimal LowerBound { get; set; }

        [DisplayName("Upper Bound")]
        decimal? UpperBound { get; set; }

        [DisplayName("Amount")]
        [Required]
        decimal BaseAmount { get; set; }
    }
}