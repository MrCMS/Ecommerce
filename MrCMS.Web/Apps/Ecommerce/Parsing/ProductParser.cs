using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ProductParser : INewsletterItemParser<Product>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ProductImage\]");
        private static readonly Regex NameRegex = new Regex(@"\[(?i)ProductName\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)ProductUrl\]");
        private static readonly Regex PriceRegex = new Regex(@"\[(?i)ProductPrice\]");
        private static readonly Regex OldPriceRegex = new Regex(@"\[(?i)ProductOldPrice\]");
        private readonly IUrlHelper _urlHelper;

        public ProductParser(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public string Parse(NewsletterTemplate template, Product item)
        {
            string output = template.ProductTemplate;
            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(item.DisplayImageUrl));
            output = NameRegex.Replace(output, item.Name ?? string.Empty);
            output = LinkRegex.Replace(output, item.AbsoluteUrl);
            output = PriceRegex.Replace(output, GetPrice(item.DisplayPrice));
            output = OldPriceRegex.Replace(output, GetPrice(item.DisplayPreviousPrice));
            return output;
        }

        private static string GetPrice(decimal? price)
        {
            return price.HasValue
                ? string.Format("{0:£0.00}", price)
                : string.Empty;
        }
    }
}