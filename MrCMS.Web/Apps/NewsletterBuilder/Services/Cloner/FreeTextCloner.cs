using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Cloner
{
    public class FreeTextCloner : ContentItemCloner<FreeText>
    {
        public override FreeText Clone(FreeText entity)
        {
            return new FreeText
            {
                Text = entity.Text
            };
        }
    }
}