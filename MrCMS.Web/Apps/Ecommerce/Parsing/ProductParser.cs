using System.Drawing;
using System.Text.RegularExpressions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.TemplateData;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder;
using MrCMS.Web.Apps.NewsletterBuilder.Services;

namespace MrCMS.Web.Apps.Ecommerce.Parsing
{
    public class ProductParser : INewsletterProductParser
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ProductImage\]");
        private static readonly Regex NameRegex = new Regex(@"\[(?i)ProductName\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)ProductUrl\]");
        private static readonly Regex PriceRegex = new Regex(@"\[(?i)ProductPrice\]");
        private static readonly Regex OldPriceRegex = new Regex(@"\[(?i)ProductOldPrice\]");
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IUrlHelper _urlHelper;

        public ProductParser(IUrlHelper urlHelper, IFileService fileService, IImageProcessor imageProcessor)
        {
            _urlHelper = urlHelper;
            _fileService = fileService;
            _imageProcessor = imageProcessor;
        }

        public string Parse(ProductListTemplateData template, Product item)
        {
            if (template == null)
                return string.Empty;

            string output = template.ProductTemplate;
            if (string.IsNullOrWhiteSpace(output))
                return string.Empty;

            MediaFile image = _imageProcessor.GetImage(item.DisplayImageUrl);
            output = ImageRegex.Replace(output,
                _urlHelper.ToAbsolute(_fileService.GetFileLocation(image, new Size { Width = 150, Height = 150 })));
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