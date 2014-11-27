using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface INopCommerceProductReader
    {
        string Name { get; }

        List<CategoryData> GetCategoryData(string connectionString);
        List<ProductOptionData> GetProductOptions(string connectionString);
        List<ProductOptionValueData> GetProductOptionValues(string connectionString);
        List<ProductSpecificationData> GetProductSpecifications(string connectionString);
        List<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions(string connectionString);
        List<ProductSpecificationValueData> GetProductSpecificationValues(string connectionString);
        List<BrandData> GetBrands(string connectionString);
        List<TagData> GetTags(string connectionString);
        List<TaxData> GetTaxData(string connectionString);
        List<CountryData> GetCountryData(string connectionString);
        List<RegionData> GetRegionData(string connectionString);
        List<ProductData> GetProducts(string connectionString);
    }
}