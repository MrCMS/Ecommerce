using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductOptionManager
    {
        IList<ProductSpecificationAttribute> ListSpecificationAttributes();
        ProductSpecificationAttribute GetSpecificationAttribute(int id);
        void AddSpecificationAttribute(ProductSpecificationAttribute option);
        void UpdateSpecificationAttribute(ProductSpecificationAttribute option);
        void DeleteSpecificationAttribute(ProductSpecificationAttribute option);

        IList<ProductSpecificationAttributeOption> ListSpecificationAttributeOptions(int id);
        void AddSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOptionDisplayOrder(IList<SortItem> options);
        void DeleteSpecificationAttributeOption(ProductSpecificationAttributeOption option);

        void AddAttributeOption(ProductAttributeOption productAttributeOption);
        void UpdateAttributeOption(ProductAttributeOption option);
        IList<ProductAttributeOption> ListAttributeOptions();
        void DeleteAttributeOption(ProductAttributeOption option);

        void SetSpecificationValue(Product product, string optionName, string value);
        void SetAttributeValue(ProductVariant productVariant, string attributeName, string value);
    }
}