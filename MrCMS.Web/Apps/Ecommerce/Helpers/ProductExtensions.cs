using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Website;

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
    }
}