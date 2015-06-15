using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Cloner
{
    public class BannerCloner : ContentItemCloner<Banner>
    {
        public override Banner Clone(Banner entity)
        {
            return new Banner
            {
                LinkUrl = entity.LinkUrl,
                ImageUrl = entity.ImageUrl
            };
        }
    }
}