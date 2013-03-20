using MrCMS.Web.Apps.Ecommerce.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IRegionService
    {
        void Add(Region region);
        void Update(Region region);
        void Delete(Region region);
    }
}