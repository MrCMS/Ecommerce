using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class TidyUpProduct : IOnDeleting<ProductVariant>
    {
        public void Execute(OnDeletingArgs<ProductVariant> args)
        {
            var productVariant = args.Item;
            var product = productVariant.Product;
            if (product != null)
            {
                product.Variants.Remove(productVariant);
                args.Session.Transact(session => session.Update(product));
            }
        }
    }
}