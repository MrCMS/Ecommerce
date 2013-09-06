using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseService
    {
        List<SelectListItem> GetGoogleCategories();
        GoogleBaseProduct GetGoogleBaseProduct(int id);
        void SaveGoogleBaseProduct(GoogleBaseProduct item);
        IPagedList<GoogleBaseCategory> Search(string queryTerm = null, int page = 1);
    }
}