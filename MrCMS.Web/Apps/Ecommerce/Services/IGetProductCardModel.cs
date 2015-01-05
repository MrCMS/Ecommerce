using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IGetProductCardModel
    {
        ProductCardModel Get(Product product);
        List<ProductCardModel> Get(List<Product> products);
        List<ProductCardModel> Get(List<int> productIds);
    }
}