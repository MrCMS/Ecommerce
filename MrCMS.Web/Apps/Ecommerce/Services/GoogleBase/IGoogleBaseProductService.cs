using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public interface IGoogleBaseProductService
    {
        GoogleBaseProduct Get(int id);
        void Add(GoogleBaseProduct item);
        void Update(GoogleBaseProduct item);
    }
}