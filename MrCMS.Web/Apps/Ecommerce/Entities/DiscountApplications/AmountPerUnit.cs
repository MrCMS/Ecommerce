using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications
{
    public class AmountPerUnit : CartItemBasedDiscountApplication
    {
        [Range(0, int.MaxValue)]
        public virtual decimal DiscountAmount { get; set; }


        public override string DisplayName
        {
            get { return string.Format("{0:0.00} per unit", DiscountAmount); }
        }
    }
}