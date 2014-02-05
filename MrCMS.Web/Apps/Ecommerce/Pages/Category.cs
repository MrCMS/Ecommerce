using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;

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

        [DisplayName("Default Product Search Sort")]
        public virtual ProductSearchSort? DefaultProductSearchSort { get; set; }

        public override void AddCustomSitemapData(UrlHelper urlHelper, XmlNode url, XmlDocument xmlDocument)
        {
            if (!string.IsNullOrEmpty(FeatureImage))
            {
                var image = url.AppendChild(xmlDocument.CreateElement("image:image"));
                var imageLoc = image.AppendChild(xmlDocument.CreateElement("image:loc"));
                imageLoc.InnerText = urlHelper.AbsoluteContent(FeatureImage);
            }
        }

        [StringLength(500)]
        public virtual string Abstract { get; set; }

        public override void AdminViewData(ViewDataDictionary viewData, NHibernate.ISession session)
        {
            viewData["product-search-sort-options"] =
                Enum.GetValues(typeof (ProductSearchSort))
                    .Cast<ProductSearchSort>()
                    .BuildSelectItemList(sort => sort.GetDescription(), sort => sort.ToString(),
                                         sort => sort == DefaultProductSearchSort, emptyItemText: "System default");
            base.AdminViewData(viewData, session);
        }
    }
}