using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Category : Webpage
    {
        public Category()
        {
            Products = new List<Product>();
        }

        private string _nestedName;

        public virtual string NestedName
        {
            get { return _nestedName ?? (_nestedName = GetNestedName()); }
        }

        private string GetNestedName()
        {
            var categories = ActivePages.TakeWhile(webpage => webpage.Unproxy() is Category).Reverse();

            return string.Join(" > ", categories.Select(webpage => webpage.Name));
        }

        public virtual IList<Product> Products { get; set; }
        public virtual string ContainerUrl
        {
            get { return (Parent as Webpage).LiveUrlSegment; }
        }

        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }

        public override void AddCustomSitemapData(UrlHelper urlHelper, XmlNode url, XmlDocument xmlDocument)
        {
            if (!string.IsNullOrEmpty(FeatureImage))
            {
                var image = url.AppendChild(xmlDocument.CreateElement("image:image"));
                var imageLoc = image.AppendChild(xmlDocument.CreateElement("image:loc"));
                imageLoc.InnerText = urlHelper.AbsoluteContent(FeatureImage);
            }
        }
    }
}