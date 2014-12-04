using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class XPercentFromItems : CartItemBasedDiscountApplication
    {
        [DisplayName("Discount % (e.g: 15)")]
        [Range(0, 100)]
        public virtual decimal DiscountPercent { get; set; }


        public override string DisplayName
        {
            get { return string.Format("{0:00}% from items", DiscountPercent); }
        }
    }
}