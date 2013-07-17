using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public abstract class ProductVariantMaxStringLength : IProductVariantImportValidationRule
    {
        protected string DisplayName { get; private set; }
        protected Func<ProductVariantImportDataTransferObject, string> Selector { get; private set; }
        protected int Length { get; private set; }

        protected ProductVariantMaxStringLength(string displayName, Func<ProductVariantImportDataTransferObject, string> selector, int length)
        {
            DisplayName = displayName;
            Selector = selector;
            Length = length;
        }

        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            var value = Selector(productVariant);
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (value.Length > Length)
                    yield return
                        string.Format(
                            "{0} is too long - max length is {1} characters and your value is {2} characters in length.",
                            DisplayName, Length, value.Length);
            }
        }
    }
}