using System.ComponentModel;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemDoesNotHaveSKU : DiscountLimitation
    {
        [DisplayName("SKUs (comma delimited list)")]
        public virtual string SKUs { get; set; }

        public override string DisplayName => "Item does not have one of the following SKUs: " + SKUs;
    }

    public class ItemBrandIsNot : DiscountLimitation
    {
        [DisplayName("Brand Names (comma delimited list)")]
        public virtual string Brands { get; set; }

        public override string DisplayName => "Product brand is not one of the following: " + Brands;
    }
}