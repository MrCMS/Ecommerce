using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public abstract class ProductVariantEqOrGrThan : IProductVariantImportValidationRule
    {
        protected string DisplayName { get; private set; }
        protected Func<ProductVariantImportDataTransferObject, decimal?> Selector { get; private set; }
        protected int Min { get; private set; }

        protected ProductVariantEqOrGrThan(string displayName, Func<ProductVariantImportDataTransferObject, decimal?> selector, int min)
        {
            DisplayName = displayName;
            Selector = selector;
            Min = min;
        }

        public IEnumerable<string> GetErrors(ProductVariantImportDataTransferObject productVariant)
        {
            var value = Selector(productVariant);
            if (value.HasValue)
            {
                if (value < Min)
                   yield return 
                       string.Format(
                        "{0} value must be greather than or equal to {1}.",
                        DisplayName,Min);
            }
        }
    }
}