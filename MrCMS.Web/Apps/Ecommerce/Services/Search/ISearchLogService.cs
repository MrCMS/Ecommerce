using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Search;

namespace MrCMS.Web.Apps.Ecommerce.Services.Search
{
    public interface ISearchLogService
    {
        SearchLog GetById(int id);
        IList<SearchLog> GetAll();
        void Add(SearchLog searchLog);
        void Update(SearchLog searchLog);
        void Delete(SearchLog searchLog);
        IPagedList<SearchLog> GetPaged(int pageNum, string search, int pageSize = 10);
    }
}