using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ImageRightAndTextLeftItemParser : INewsletterItemParser<ImageRightAndTextLeft>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly IUrlHelper _urlHelper;

        public ImageRightAndTextLeftItemParser(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Parse(NewsletterTemplate template, ImageRightAndTextLeft item)
        {
            string output = template.ImageRightAndTextLeftTemplate;
            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }

    public class ImageLeftAndTextRightParser : INewsletterItemParser<ImageLeftAndTextRight>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly IUrlHelper _urlHelper;

        public ImageLeftAndTextRightParser(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Parse(NewsletterTemplate template, ImageLeftAndTextRight item)
        {
            string output = template.ImageLeftAndTextRightTemplate;
            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}