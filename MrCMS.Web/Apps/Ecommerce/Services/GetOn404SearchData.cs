using System.Text.RegularExpressions;
using System.Web;
using MrCMS.Paging;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Widgets;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetOn404SearchData:GetWidgetModelBase<On404SearchWidget>
    {
        private readonly HttpRequestBase _request;
        private readonly IProductSearchIndexService _productSearchIndexService;

        public GetOn404SearchData(HttpRequestBase request, IProductSearchIndexService productSearchIndexService)
        {
            _request = request;
            _productSearchIndexService = productSearchIndexService;
        }

        public override object GetModel(On404SearchWidget widget)
        {
            var path = _request.Url.AbsolutePath;

            IPagedList<Product> searchProducts = _productSearchIndexService.SearchProducts(new ProductSearchQuery
            {
                SearchTerm = GetSearchTerm(path),
                PageSize = widget.MaxProductsToShow,
                Page = 1
            });
            return new On404SearchWidgetModel
            {
                Text = widget.Text,
                Products = searchProducts.GetCardModels()
            };
        }

        private string GetSearchTerm(string path)
        {
            path = (path ?? string.Empty);
            path = Regex.Replace(path, "[^a-zA-Z0-9]", " ");
            return Regex.Replace(path, @"\s+", " ").Trim();
        }
    }
}