using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ImageAndTextItemParser : INewsletterItemParser<ImageAndText>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly IUrlHelper _urlHelper;

        public ImageAndTextItemParser(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Parse(NewsletterTemplate template, ImageAndText item)
        {
            string output = template.ImageAndTextTemplate;
            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}