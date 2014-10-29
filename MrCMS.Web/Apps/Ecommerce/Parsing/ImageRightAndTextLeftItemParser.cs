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
        private readonly INewsletterUrlHelper _newsletterUrlHelper;

        public ImageRightAndTextLeftItemParser(INewsletterUrlHelper newsletterUrlHelper)
        {
            _newsletterUrlHelper = newsletterUrlHelper;
        }

        public string Parse(NewsletterTemplate template, ImageRightAndTextLeft item)
        {
            string output = template.ImageRightAndTextLeftTemplate;
            output = ImageRegex.Replace(output, _newsletterUrlHelper.ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }

    public class ImageLeftAndTextRightParser : INewsletterItemParser<ImageLeftAndTextRight>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly INewsletterUrlHelper _newsletterUrlHelper;

        public ImageLeftAndTextRightParser(INewsletterUrlHelper newsletterUrlHelper)
        {
            _newsletterUrlHelper = newsletterUrlHelper;
        }

        public string Parse(NewsletterTemplate template, ImageLeftAndTextRight item)
        {
            string output = template.ImageLeftAndTextRightTemplate;
            output = ImageRegex.Replace(output, _newsletterUrlHelper.ToAbsolute(item.ImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}