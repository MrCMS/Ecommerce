using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductCategoriesExist : IProductImportValidationRule
    {
        private readonly IDocumentService _documentService;

        public ProductCategoriesExist(IDocumentService documentService)
        {
            _documentService = documentService; 
        }

        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            return (from item in product.Categories
                    where _documentService.GetDocumentByUrl<Category>(item) == null
                    select string.Format("Category with url: {0} is not present within the system.", item)).ToList();
        }
    }
}