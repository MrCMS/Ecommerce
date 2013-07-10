using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport.Rules
{
    public abstract class ProductMaxStringLength : IProductImportValidationRule
    {
        protected string DisplayName { get; private set; }
        protected Func<ProductImportDataTransferObject, string> Selector { get; private set; }
        protected int Length { get; private set; }

        protected ProductMaxStringLength(string displayName, Func<ProductImportDataTransferObject,string> selector, int length)
        {
            DisplayName = displayName;
            Selector = selector;
            Length = length;
        }

        public IEnumerable<string> GetErrors(ProductImportDataTransferObject product)
        {
            var value = Selector(product);
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