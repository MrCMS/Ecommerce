using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductCategoriesExist : IProductImportValidationRule
    {
        private readonly ICategoryService _categoryService;

        public ProductCategoriesExist(ICategoryService categoryService)
        {
            _categoryService = categoryService; 
        }

        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            return (from item in product.Categories
                    where _categoryService.Get(item) == null
                    select string.Format("Category with Id: {0} is not present within the system.", item)).ToList();
        }
    }
}