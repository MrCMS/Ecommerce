using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ImageAndTextItemParser : AbstractNewsletterItemParser<ImageAndText>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");

        public override string Parse(NewsletterTemplate template, ImageAndText item)
        {
            string output = template.ImageAndTextTemplate;
            output = ImageRegex.Replace(output, ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}