using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.DiscountLimitations
{
    public class OrderHasXOrMoreItems : DiscountLimitation
    {
        public override bool IsCartValid(CartModel cartModel)
        {
            var itemQuantity = cartModel.Items.Any() ? cartModel.Items.Sum(item => item.Quantity) : 0;
            return itemQuantity >= NumberOfItems;
        }

        public override bool IsItemValid(CartItem cartItem)
        {
            return false;
        }

        public virtual decimal NumberOfItems { get; set; }

        public override void CopyValues(DiscountLimitation limitation)
        {
            this.NumberOfItems = ((OrderHasXOrMoreItems)limitation).NumberOfItems;
        }
    }
}