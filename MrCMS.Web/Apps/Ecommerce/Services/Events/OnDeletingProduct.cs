using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Events
{
    public class OnDeletingProduct:IOnDeleting<Product>
    {
        public void Execute(OnDeletingArgs<Product> args)
        {
            var product = args.Item;
            if (product.Categories.Count > 0)
            {
                foreach (var category in product.Categories)
                    category.Products.Remove(product);
                product.Categories.Clear();
            }
        }
    }
}