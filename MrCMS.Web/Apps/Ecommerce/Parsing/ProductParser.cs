using System.Drawing;
using System.Text.RegularExpressions;
using MrCMS.Services;
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
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;

        public ProductParser(IUrlHelper urlHelper, IFileService fileService, IImageProcessor imageProcessor)
        {
            _urlHelper = urlHelper;
            _fileService = fileService;
            _imageProcessor = imageProcessor;
        }

        public string Parse(NewsletterTemplate template, Product item)
        {
            string output = template.ProductTemplate;
            var image = _imageProcessor.GetImage(item.DisplayImageUrl);

            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(_fileService.GetFileLocation(image, new Size{Width = 200, Height = 200})));
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