using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models
{
    public class CalculationInfo
    {
        public ShippingCriteria ShippingCriteria { get; set; }

        public decimal LowerBound { get; set; }

        public decimal? UpperBound { get; set; }

        public int? Id { get; set; }
    }
}