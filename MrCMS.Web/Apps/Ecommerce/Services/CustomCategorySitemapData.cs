using System;
using System.Web.Mvc;
using System.Xml.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Sitemaps;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CustomCategorySitemapData : SitemapGenerationInfo<Category>
    {
        private readonly UrlHelper _urlHelper;

        public CustomCategorySitemapData(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override bool ShouldAppend(Category webpage)
        {
            return webpage.Published;
        }

        public override void Append(Category webpage, XElement urlset, XDocument xmlDocument)
        {
            DateTime? publishOn = webpage.PublishOn;
            if (!publishOn.HasValue)
                return;
            var urlNode = new XElement(SitemapService.RootNamespace + "url",
                new XElement(SitemapService.RootNamespace + "loc", webpage.AbsoluteUrl),
                new XElement(SitemapService.RootNamespace + "lastmod", webpage.PublishOn.Value.ToString("O"))
                );

            if (!string.IsNullOrWhiteSpace(webpage.FeatureImage))
                urlNode.Add(new XElement(SitemapService.ImageNameSpace + "image",
                    new XElement(SitemapService.ImageNameSpace + "loc", _urlHelper.AbsoluteContent(webpage.FeatureImage))));

            urlset.Add(urlNode);
        }
    }
}