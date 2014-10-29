using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class BannerItemParser : INewsletterItemParser<Banner>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)LinkUrl\]");
        private readonly INewsletterUrlHelper _newsletterUrlHelper;

        public BannerItemParser(INewsletterUrlHelper newsletterUrlHelper)
        {
            _newsletterUrlHelper = newsletterUrlHelper;
        }

        public string Parse(NewsletterTemplate template, Banner item)
        {
            string output = template.BannerTemplate;
            output = ImageRegex.Replace(output, _newsletterUrlHelper.ToAbsolute(item.ImageUrl));
            output = LinkRegex.Replace(output, _newsletterUrlHelper.ToAbsolute(item.LinkUrl));
            return output;
        }
    }
}