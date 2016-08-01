using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemIsInCategory : DiscountLimitation
    {
        [DisplayName("Category IDs (comma delimited list)")]
        public virtual string CategoryIds { get; set; }

        [DisplayName("Category Names (comma delimited list)")]
        public virtual string CategoryNames { get; set; }

        public override string DisplayName
        {
            get { return "Item has one of the following categories: " + CategoryNames; }
        }
    }
}