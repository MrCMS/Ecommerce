using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductService
    {
        ProductPagedList Search(string queryTerm = null, int page = 1);
    }
}