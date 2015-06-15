using System.Text.RegularExpressions;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.TemplateData;

namespace MrCMS.Web.Apps.NewsletterBuilder.Services.Parsing
{
    public class FreeTextItemParser : INewsletterItemParser<FreeText>
    {
        private static readonly Regex TextRegex = new Regex(@"\[(?i)Text\]");
        private readonly IGetContentItemTemplateData _getContentItemTemplateData;

        public FreeTextItemParser(IGetContentItemTemplateData getContentItemTemplateData)
        {
            _getContentItemTemplateData = getContentItemTemplateData;
        }

        public string Parse(NewsletterTemplate template, FreeText item)
        {
            var freeTextTemplateData = _getContentItemTemplateData.Get<FreeTextTemplateData>(template);
            if (freeTextTemplateData == null)
                return string.Empty;
            string output = freeTextTemplateData.FreeTextTemplate;
            if (string.IsNullOrWhiteSpace(output))
                return output;
            output = TextRegex.Replace(output, item.Text ?? string.Empty);
            return output;
        }
    }
}