using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Linq;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ProductBuilder
    {
        private List<Category> _categories = new List<Category>();

        public Product Build()
        {
            return new Product
                {
                    Categories = _categories
                };
        }

        public ProductBuilder WithCategories(params Category[] categories)
        {
            _categories = categories.ToList();
            return this;
        }
    }
}