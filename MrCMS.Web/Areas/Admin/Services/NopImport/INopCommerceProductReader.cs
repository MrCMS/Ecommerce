using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface INopCommerceProductReader
    {
        List<CategoryData> GetCategoryData(string connectionString);
        List<ProductOptionData> GetProductOptions(string connectionString);
        List<ProductOptionValueData> GetProductOptionValues(string connectionString);
        List<ProductSpecificationData> GetProductSpecifications(string connectionString);
        List<ProductSpecificationValueData> GetProductSpecificationValues(string connectionString);
        List<BrandData> GetBrands(string connectionString);
        List<ProductData> GetProducts(string connectionString);
    }
}