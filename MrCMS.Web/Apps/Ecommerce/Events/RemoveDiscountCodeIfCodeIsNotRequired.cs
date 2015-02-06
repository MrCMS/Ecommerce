using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class RemoveDiscountCodeIfCodeIsNotRequired : IOnAdding<Discount>, IOnUpdating<Discount>
    {
        public void Execute(OnAddingArgs<Discount> args)
        {
            RemoveCodeIfRequired(args.Item);
        }

        public void Execute(OnUpdatingArgs<Discount> args)
        {
            RemoveCodeIfRequired(args.Item);
        }

        private void RemoveCodeIfRequired(Discount item)
        {
            if (!item.RequiresCode)
                item.Code = null;
        }
    }
}