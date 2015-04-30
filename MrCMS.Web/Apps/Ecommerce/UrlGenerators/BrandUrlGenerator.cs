using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.UrlGenerators
{
    public class BrandUrlGenerator : WebpageUrlGenerator<Brand>
    {
        private readonly EcommerceSettings _settings;

        public BrandUrlGenerator(EcommerceSettings settings)
        {
            _settings = settings;
        }

        public override string GetUrl(string pageName, Webpage parent, bool useHierarchy)
        {
            var brandUrl = _settings.BrandUrl ?? "{0}";

            var stringBuilder = new StringBuilder();

            if (useHierarchy && parent != null)
            {
                stringBuilder.Insert(0, SeoHelper.TidyUrl(parent.UrlSegment) + "/");
            }

            stringBuilder.Append(string.Format(brandUrl, SeoHelper.TidyUrl(pageName)));

            return stringBuilder.ToString();
        }
    }
}