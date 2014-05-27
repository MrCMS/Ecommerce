using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class BannerItemParser : AbstractNewsletterItemParser<Banner>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)LinkUrl\]");

        public override string Parse(NewsletterTemplate template, Banner item)
        {
            string output = template.BannerTemplate;
            output = ImageRegex.Replace(output, ToAbsolute(item.ImageUrl));
            output = LinkRegex.Replace(output, ToAbsolute(item.LinkUrl));
            return output;
        }
    }
}