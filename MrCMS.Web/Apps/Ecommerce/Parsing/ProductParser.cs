using System.Text.RegularExpressions;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ProductParser : AbstractNewsletterItemParser<Product>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ProductImage\]");
        private static readonly Regex NameRegex = new Regex(@"\[(?i)ProductName\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)ProductUrl\]");
        private static readonly Regex PriceRegex = new Regex(@"\[(?i)ProductPrice\]");
        private static readonly Regex OldPriceRegex = new Regex(@"\[(?i)ProductOldPrice\]");

        public override string Parse(NewsletterTemplate template, Product item)
        {
            string output = template.ProductTemplate;
            output = ImageRegex.Replace(output, ToAbsolute(item.DisplayImageUrl));
            output = NameRegex.Replace(output, item.Name ?? string.Empty);
            output = LinkRegex.Replace(output, ToAbsolute(item.LiveUrlSegment));
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