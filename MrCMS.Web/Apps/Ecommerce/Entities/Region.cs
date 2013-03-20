using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities
{
    public class Region : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual Country Country { get; set; }
    }
}