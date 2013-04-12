using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Geographic
{
    public interface IRegionService
    {
        Region Get(int regionId);
        void Add(Region region);
        void Update(Region region);
        void Delete(Region region);
        object GetRegionsByCountryId(int countryId);
    }
}