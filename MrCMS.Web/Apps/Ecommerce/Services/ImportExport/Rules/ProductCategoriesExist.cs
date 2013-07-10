using System;
using System.Collections.Generic;
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
            var errors=new List<string>();
            foreach (var item in product.Categories)
            {
                if(_categoryService.Get(item)==null)
                    errors.Add(string.Format(
                        "Category with Id: {0} is not present within the system.",
                        item));
            }
            return errors;
        }
    }
}