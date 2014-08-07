using System.Web.Mvc;
using System.Xml;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class CustomCategorySitemapData:CustomSiteMapBase<Category>
    {
        private readonly UrlHelper _urlHelper;

        public CustomCategorySitemapData(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override void AddCustomSiteMapData(Category webpage, XmlNode mainNode, XmlDocument document)
        {
            if (!string.IsNullOrEmpty(webpage.FeatureImage))
            {
                var image = mainNode.AppendChild(document.CreateElement("image:image"));
                var imageLoc = image.AppendChild(document.CreateElement("image:loc"));
                imageLoc.InnerText = _urlHelper.AbsoluteContent(webpage.FeatureImage);
            }
        }
    }
}