using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public interface INopCommerceDataReader
    {
        string Name { get; }
        void SetConnectionString(string connectionString);
        void SetPictureInfo(PictureInfo pictureInfo);

        HashSet<CategoryData> GetCategoryData();
        HashSet<ProductOptionData> GetProductOptions();
        HashSet<ProductOptionValueData> GetProductOptionValues();
        HashSet<ProductSpecificationData> GetProductSpecifications();
        HashSet<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions();
        HashSet<ProductSpecificationValueData> GetProductSpecificationValues();
        HashSet<BrandData> GetBrands();
        HashSet<TagData> GetTags();
        HashSet<TaxData> GetTaxData();
        HashSet<CountryData> GetCountryData();
        HashSet<RegionData> GetRegionData();
        HashSet<ProductData> GetProducts();
        HashSet<UserData> GetUserData();
        HashSet<OrderData> GetOrderData();
        HashSet<OrderLineData> GetOrderLineData();
        HashSet<AddressData> GetAddressData();
    }
}