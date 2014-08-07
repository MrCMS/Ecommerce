using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.Events
{
    public class OnDeletingCategory : IOnDeleting<Category>
    {
        public void Execute(OnDeletingArgs<Category> args)
        {
            var category = args.Item;
            if (category.Products.Count > 0)
            {
                foreach (var product in category.Products)
                    product.Categories.Remove(category);

                category.Products.Clear();
            }
        }
    }
}