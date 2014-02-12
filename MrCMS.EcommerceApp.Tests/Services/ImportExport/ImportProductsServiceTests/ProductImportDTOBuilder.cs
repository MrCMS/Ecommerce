using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.EcommerceApp.Tests.Services.ImportExport.ImportProductsServiceTests
{
    public class ProductImportDTOBuilder
    {
        private List<string> _categories = new List<string>();

        public ProductImportDataTransferObject Build()
        {
            return new ProductImportDataTransferObject
                {
                    Categories = _categories
                };
        }

        public ProductImportDTOBuilder WithCategories(params string[] categories)
        {
            _categories = categories.ToList();
            return this;
        }

        public ProductImportDTOBuilder WithNoCategories()
        {
            _categories = new List<string>();
            return this;
        }
    }
}