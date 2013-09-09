using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseManager
    {
        void SaveGoogleBaseProduct(GoogleBaseProduct item);
        IPagedList<GoogleBaseCategory> SearchGoogleBaseCategories(string queryTerm = null, int page = 1);
        byte[] ExportProductsToGoogleBase();
    }
}