using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers
{
    public static class EntityExtensions 
    {
        public static void AssignBaseProperties<T>(this T siteEntity, Site site) where T : SiteEntity
        {
            var now = CurrentRequestData.Now;
            siteEntity.CreatedOn = now;
            siteEntity.Site = site;
            siteEntity.UpdatedOn = now;
        }
        public static void AssignBaseProperties<T>(this T systemEntity) where T : SystemEntity
        {
            var now = CurrentRequestData.Now;
            systemEntity.CreatedOn = now;
            systemEntity.UpdatedOn = now;
        }
    }
}