using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Cloner
{
    public class ImageLeftAndTextRightCloner : ContentItemCloner<ImageLeftAndTextRight>
    {
        public override ImageLeftAndTextRight Clone(ImageLeftAndTextRight entity)
        {
            return new ImageLeftAndTextRight
            {
                Text = entity.Text,
                ImageUrl = entity.ImageUrl
            };
        }
    }
}