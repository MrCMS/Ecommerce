using MrCMS.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class SetupProductVariant :IOnAdding<ProductVariant>
    {
        public void Execute(OnAddingArgs<ProductVariant> args)
        {
            ProductVariant productVariant = args.Item;
            if (productVariant.Product != null)
            {
                productVariant.Product.Variants.Add(productVariant);

                foreach (ProductOptionValue value in productVariant.OptionValues)
                {
                    value.ProductVariant = productVariant;
                }
            }
        }
    }
}