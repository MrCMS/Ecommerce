using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Events
{
    public class UpdateProductIndexesWhenDisplayOrderSet : IOnAdding<CategoryProductDisplayOrder>,
        IOnUpdating<CategoryProductDisplayOrder>, IOnDeleting<CategoryProductDisplayOrder>
    {
        public void Execute(OnAddingArgs<CategoryProductDisplayOrder> args)
        {
            UpdateProduct(args.Session, args.Item.Product);
        }

        public void Execute(OnDeletingArgs<CategoryProductDisplayOrder> args)
        {
            UpdateProduct(args.Session, args.Item.Product);
        }

        public void Execute(OnUpdatingArgs<CategoryProductDisplayOrder> args)
        {
            UpdateProduct(args.Session, args.Item.Product);
        }

        private void UpdateProduct(ISession session, Product product)
        {
            session.Transact(s => s.Update(product));
        }
    }
}