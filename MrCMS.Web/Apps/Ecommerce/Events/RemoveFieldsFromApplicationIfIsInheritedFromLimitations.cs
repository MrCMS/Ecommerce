using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.DiscountApplications;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class RemoveFieldsFromApplicationIfIsInheritedFromLimitations :
        IOnAdding<CartItemBasedDiscountApplication>, IOnUpdating<CartItemBasedDiscountApplication>
    {
        public void Execute(OnAddingArgs<CartItemBasedDiscountApplication> args)
        {
            RemoveSKUAndCategoryIfRequired(args.Item);
        }

        public void Execute(OnUpdatingArgs<CartItemBasedDiscountApplication> args)
        {
            RemoveSKUAndCategoryIfRequired(args.Item);
        }

        private void RemoveSKUAndCategoryIfRequired(CartItemBasedDiscountApplication item)
        {
            if (item.CartItemsFromLimitations)
            {
                item.CategoryIds = null;
                item.SKUs = null;
            }
        }
    }
}