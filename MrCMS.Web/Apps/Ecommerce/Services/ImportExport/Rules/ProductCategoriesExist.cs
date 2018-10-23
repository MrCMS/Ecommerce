using System.Collections.Generic;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductCategoriesExist : IProductImportValidationRule
    {
        private readonly IGetDocumentByUrl<Category> _documentService;

        public ProductCategoriesExist(IGetDocumentByUrl<Category> documentService)
        {
            _documentService = documentService;
        }

        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            return (from item in product.Categories
                where _documentService.GetByUrl(item) == null
                select string.Format("Category with url: {0} is not present within the system.", item)).ToList();
        }
    }
}