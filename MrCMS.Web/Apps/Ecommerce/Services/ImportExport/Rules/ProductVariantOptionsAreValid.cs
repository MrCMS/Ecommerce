using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public class ProductVariantOptionsAreValid : IProductVariantImportValidationRule
    {
        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            var errors=new List<string>();
            foreach (var item in productVariant.Options)
            {
                if (item.Key.Length > 255)
                    errors.Add(string.Format(
                        "{0} is too long - max length is {1} characters and your value is {2} characters in length",
                        item.Key + " Name", 255, item.Key.Length));
                if (item.Value.Length > 255)
                    errors.Add(string.Format(
                        "{0} is too long - max length is {1} characters and your value is {2} characters in length",
                        item.Key + " Value", 255, item.Value.Length));
            }
            return errors;
        }
    }
}