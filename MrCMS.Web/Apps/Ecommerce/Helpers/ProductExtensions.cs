using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Nop280;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website;
using NHibernate;
using Product = MrCMS.Web.Apps.Ecommerce.Pages.Product;

namespace MrCMS.Web.Apps.Ecommerce.Helpers
{
    public static class ProductExtensions
    {
        public static ProductCardModel GetCardModel(this Product product)
        {
            return MrCMSApplication.Get<IGetProductCardModel>().Get(product);
        }
        public static List<ProductCardModel> GetCardModels(this IEnumerable<Product> product)
        {
            return MrCMSApplication.Get<IGetProductCardModel>().Get(product.ToList());
        }
        public static List<ProductCardModel> GetCardModels(this IEnumerable<int> productIds)
        {
            return MrCMSApplication.Get<IGetProductCardModel>().Get(productIds.ToList());
        }
        public static int GetVariantCount(this Product product)
        {
            if (product == null)
                return 0;
            var session = MrCMSApplication.Get<ISession>();
            return
                session.QueryOver<ProductVariant>()
                    .Where(variant => variant.Product.Id == product.Id)
                    .Cacheable()
                    .RowCount();
        }
    }
}