using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class XPercentFromOrder : DiscountApplication
    {
        [DisplayName("Discount % (e.g: 15)")]
        [Range(0, 100)]
        public virtual decimal DiscountPercent { get; set; }

        public override string DisplayName
        {
            get { return string.Format("{0:0.00}% from order total", DiscountPercent); }
        }
    }
}