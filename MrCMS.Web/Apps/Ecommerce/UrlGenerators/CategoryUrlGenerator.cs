using System.Text;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;

namespace MrCMS.Web.Apps.Ecommerce.UrlGenerators
{
    public class CategoryUrlGenerator : WebpageUrlGenerator<Category>
    {
        private readonly EcommerceSettings _settings;

        public CategoryUrlGenerator(EcommerceSettings settings)
        {
            _settings = settings;
        }

        public override string GetUrl(string pageName, Webpage parent, bool useHierarchy)
        {
            var categoryUrl = _settings.CategoryUrl ?? "{0}";

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Format(categoryUrl, SeoHelper.TidyUrl(pageName)));

            return stringBuilder.ToString();
        }
    }
}