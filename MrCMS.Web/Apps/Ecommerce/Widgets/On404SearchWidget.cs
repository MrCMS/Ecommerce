using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Widget;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class On404SearchWidget : Widget
    {
        public On404SearchWidget()
        {
            MaxProductsToShow = 12;
        }
        [AllowHtml]
        public virtual string Text { get; set; }

        public virtual int MaxProductsToShow { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            var requestBase = MrCMSApplication.Get<HttpRequestBase>();
            var path = requestBase.Url.AbsolutePath;

            var productSearchService = MrCMSApplication.Get<IProductSearchService>();
            return new On404SearchWidgetModel
                       {
                           Text = Text,
                           Products = productSearchService.SearchProducts(new ProductSearchQuery
                                                                              {
                                                                                  SearchTerm = GetSearchTerm(path),
                                                                                  PageSize = MaxProductsToShow,
                                                                                  Page = 1
                                                                              })
                       };
        }

        public class On404SearchWidgetModel
        {
            public string Text { get; set; }

            public bool AnyProducts { get { return Products != null && Products.Any(); } }

            public IPagedList<Product> Products { get; set; }
        }

        private string GetSearchTerm(string path)
        {
            path = (path ?? string.Empty);
            path = Regex.Replace(path, "[^a-zA-Z0-9]", " ");
            return Regex.Replace(path, @"\s+", " ").Trim();
        }
    }
}