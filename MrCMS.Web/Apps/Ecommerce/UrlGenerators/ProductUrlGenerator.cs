using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.UrlGenerators
{
    public class ProductUrlGenerator : WebpageUrlGenerator<Product>
    {
        private readonly EcommerceSettings _settings;

        public ProductUrlGenerator(EcommerceSettings settings)
        {
            _settings = settings;
        }

        public override string GetUrl(string pageName, Webpage parent, bool useHierarchy)
        {
            var productUrl = _settings.ProductUrl ?? "{0}";

            var stringBuilder = new StringBuilder();

            if (useHierarchy && parent != null)
            {
                stringBuilder.Insert(0, SeoHelper.TidyUrl(parent.UrlSegment) + "/");
            }

            stringBuilder.Append(string.Format(productUrl, SeoHelper.TidyUrl(pageName)));

            return stringBuilder.ToString();
        }
    }
}