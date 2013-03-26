using MrCMS.Web.Apps.Ecommerce.Entities;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Ecommerce.Services
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