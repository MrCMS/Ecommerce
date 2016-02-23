using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Search
{
    public interface IGetProductSearchView
    {
        ProductSearchView Get();
        void Set(ProductSearchView productSearchView);
    }
}