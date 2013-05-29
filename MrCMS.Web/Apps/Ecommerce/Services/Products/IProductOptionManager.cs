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
        bool AnyExistingAtrributesWithName(string name);

        IList<ProductSpecificationAttributeOption> ListSpecificationAttributeOptions(int id);
        void AddSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOptionDisplayOrder(IList<SortItem> options);
        void DeleteSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        bool AnyExistingAtrributeOptionsWithName(string name, int id);

        void AddAttributeOption(ProductAttributeOption productAttributeOption);
        void UpdateAttributeOption(ProductAttributeOption option);
        IList<ProductAttributeOption> ListAttributeOptions();
        void DeleteAttributeOption(ProductAttributeOption option);

        void SetAttributeValue(ProductVariant productVariant, string attributeName, string value);
        void SetSpecificationValue(Product product, ProductSpecificationAttribute productSpecificationAttribute, string Value);
        void DeleteSpecificationValue(ProductSpecificationValue specification);
        void UpdateSpecificationValueDisplayOrder(IList<SortItem> options);
        ProductSpecificationValue GetSpecificationValue(int id);
    }
}