using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Cloner
{
    public class ImageRightAndTextLeftCloner : ContentItemCloner<ImageRightAndTextLeft>
    {
        public override ImageRightAndTextLeft Clone(ImageRightAndTextLeft entity)
        {
            return new ImageRightAndTextLeft
            {
                Text = entity.Text,
                ImageUrl = entity.ImageUrl
            };
        }
    }
}