using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductImagesHaveValidUrl : IProductImportValidationRule
    {
        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            var errors=new List<string>();
            try
            {
                foreach (var item in product.Images)
                {
                    Uri testUri;
                    bool result = Uri.TryCreate(item, UriKind.Absolute, out testUri) &&
                        testUri.Scheme == Uri.UriSchemeHttp;
                    if (!result)
                        errors.Add(string.Format(
                           "{0} is not valid Url.",
                           item));
                }
            }
            catch (Exception)
            {
                errors.Add("Some of Image Urls are not in correct format.");
            }
            
            return errors;
        }
    }
}