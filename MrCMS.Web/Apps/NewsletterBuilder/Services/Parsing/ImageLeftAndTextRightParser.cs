using System.Drawing;
using System.Text.RegularExpressions;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Parsing
{
    public class ImageLeftAndTextRightParser : INewsletterItemParser<ImageLeftAndTextRight>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly IUrlHelper _urlHelper;
        private readonly IGetContentItemTemplateData _getContentItemTemplateData;
        private readonly INewsletterBuilderHelperService _newsletterBuilderHelperService;

        public ImageLeftAndTextRightParser(IUrlHelper urlHelper, IGetContentItemTemplateData getContentItemTemplateData, INewsletterBuilderHelperService newsletterBuilderHelperService)
        {
            _urlHelper = urlHelper;
            _getContentItemTemplateData = getContentItemTemplateData;
            _newsletterBuilderHelperService = newsletterBuilderHelperService;
        }

        public string Parse(NewsletterTemplate template, ImageLeftAndTextRight item)
        {
            var templateData = _getContentItemTemplateData.Get<ImageLeftAndTextRightTemplateData>(template);
            if (templateData == null)
                return string.Empty;

            string output = templateData.ImageLeftAndTextRightTemplate;
            if (string.IsNullOrWhiteSpace(output))
                return string.Empty;

            var resizedImageUrl = _newsletterBuilderHelperService.GetResizedImage(item.ImageUrl, new Size { Width = 151 });
            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(resizedImageUrl));
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}