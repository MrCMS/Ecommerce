using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class CategoryBuilder
    {
        private string _name = "Category";
        private string _urlSegment = "test-builder";

        public Category Build()
        {
            return new Category
                {
                    Name = _name,
                    UrlSegment = _urlSegment
                };
        }
        public CategoryBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CategoryBuilder WithUrlSegment(string urlSegment)
        {
            _urlSegment = urlSegment;
            return this;
        }
    }
}