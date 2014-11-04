using System.ComponentModel;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class CartHasAtLeastXItems : DiscountLimitation
    {
        public override string DisplayName
        {
            get { return "Cart has at least x items"; }
        }

        public override bool IsCartValid(CartModel cartModel)
        {
            var itemQuantity = cartModel.Items.Any() ? cartModel.Items.Sum(item => item.Quantity) : 0;
            return itemQuantity >= NumberOfItems;
        }

        public override bool IsItemValid(CartItem cartItem)
        {
            return false;
        }

        [DisplayName("Number of items")]
        public virtual int NumberOfItems { get; set; }

        public override void CopyValues(DiscountLimitation limitation)
        {
            NumberOfItems = ((CartHasAtLeastXItems)limitation).NumberOfItems;
        }
    }
}