using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class ItemIsInCategory : DiscountLimitation
    {
        public virtual int CategoryId { get; set; }
        public override string DisplayName
        {
            get { return "Item is in category"; }
        }

        public override bool IsCartValid(CartModel cartModel)
        {
            return false;
        }

        public override bool IsItemValid(CartItem cartItem)
        {
            return cartItem.Item.Product.Categories.Select(category => category.Id).Contains(CategoryId);
        }

        public override void CopyValues(DiscountLimitation limitation)
        {
            CategoryId = ((ItemIsInCategory)limitation).CategoryId;
        }
    }
}