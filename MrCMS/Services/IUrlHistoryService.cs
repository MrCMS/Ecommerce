using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUrlHistoryService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
        UrlHistory GetByUrlSegment(string urlSegment);
    }
}