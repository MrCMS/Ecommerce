using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public interface IProductOptionManager
    {
        IList<ProductSpecificationOption> ListSpecificationOptions();
        void AddSpecificationOption(ProductSpecificationOption option);
        void UpdateSpecificationOption(ProductSpecificationOption option);
        void DeleteSpecificationOption(ProductSpecificationOption option);
        void AddAttributeOption(ProductAttributeOption productAttributeOption);
        void UpdateAttributeOptionn(ProductAttributeOption option);
        IList<ProductAttributeOption> ListAttributeOptions();
        void DeleteAttributeOption(ProductAttributeOption option);
        void SetSpecificationValue(Product product, string optionName, string value);
        void SetAttributeValue(ProductVariant productVariant, string attributeName, string value);
    }
}