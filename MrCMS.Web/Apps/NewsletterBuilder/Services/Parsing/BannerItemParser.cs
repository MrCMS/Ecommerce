using System.Text.RegularExpressions;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Parsing
{
    public class BannerItemParser : INewsletterItemParser<Banner>
    {
        private static readonly Regex ImageRegex = new Regex(@"\[(?i)ImageUrl\]");
        private static readonly Regex LinkRegex = new Regex(@"\[(?i)LinkUrl\]");
        private readonly IUrlHelper _urlHelper;
        private readonly IGetContentItemTemplateData _getContentItemTemplateData;

        public BannerItemParser(IUrlHelper urlHelper,IGetContentItemTemplateData getContentItemTemplateData)
        {
            _urlHelper = urlHelper;
            _getContentItemTemplateData = getContentItemTemplateData;
        }

        public string Parse(NewsletterTemplate template, Banner item)
        {
            var bannerTemplateData = _getContentItemTemplateData.Get<BannerTemplateData>(template);
            if (bannerTemplateData == null)
                return string.Empty;

            string output = bannerTemplateData.BannerTemplate;
            if (string.IsNullOrWhiteSpace(output))
                return string.Empty;

            output = ImageRegex.Replace(output, _urlHelper.ToAbsolute(item.ImageUrl));
            output = LinkRegex.Replace(output, _urlHelper.ToAbsolute(item.LinkUrl));
            return output;
        }
    }
}