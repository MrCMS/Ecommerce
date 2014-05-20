using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IProductOptionManager
    {
        IList<ProductSpecificationAttribute> ListSpecificationAttributes();
        ProductSpecificationAttribute GetSpecificationAttribute(int id);
        ProductSpecificationAttribute GetSpecificationAttributeByName(string name);
        void AddSpecificationAttribute(ProductSpecificationAttribute option);
        void UpdateSpecificationAttribute(ProductSpecificationAttribute option);
        void DeleteSpecificationAttribute(ProductSpecificationAttribute option);
        bool AnyExistingSpecificationAttributesWithName(UniqueAttributeNameModel model);
        void UpdateSpecificationAttributeDisplayOrder(IList<SortItem> options);

        IList<ProductSpecificationAttributeOption> ListSpecificationAttributeOptions(int id);
        void AddSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        void UpdateSpecificationAttributeOptionDisplayOrder(IList<SortItem> options);
        void DeleteSpecificationAttributeOption(ProductSpecificationAttributeOption option);
        bool AnyExistingSpecificationAttributeOptionsWithName(string name, int id);

        void SetSpecificationValue(Product product, ProductSpecificationAttribute productSpecificationAttribute, string value);
        void DeleteSpecificationValue(ProductSpecificationValue specification);
        void UpdateSpecificationValueDisplayOrder(IList<SortItem> options);
        ProductSpecificationValue GetSpecificationValue(int id);

        ProductOption GetAttributeOptionByName(string name);
        void AddAttributeOption(ProductOption productOption);
        void UpdateAttributeOptionDisplayOrder(Product product, IList<SortItem> options);
        void DeleteProductAttributeValue(ProductOptionValue value);
        
        List<ProductOptionModel> GetSearchAttributeOptions(ProductSearchQuery query);
        List<ProductOptionModel> GetSearchSpecificationAttributes(ProductSearchQuery query);
    }
}