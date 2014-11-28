using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport
{
    public interface INopCommerceDataReader
    {
        string Name { get; }
        void SetConnectionString(string connectionString);
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
    }

    public abstract class NopCommerceDataReader : INopCommerceDataReader
    {
        protected string ConnectionString { get; set; }

        public abstract string Name { get; }

        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public abstract HashSet<CategoryData> GetCategoryData();
        public abstract HashSet<ProductOptionData> GetProductOptions();
        public abstract HashSet<ProductOptionValueData> GetProductOptionValues();
        public abstract HashSet<ProductSpecificationData> GetProductSpecifications();
        public abstract HashSet<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions();
        public abstract HashSet<ProductSpecificationValueData> GetProductSpecificationValues();
        public abstract HashSet<BrandData> GetBrands();
        public abstract HashSet<TagData> GetTags();
        public abstract HashSet<TaxData> GetTaxData();
        public abstract HashSet<CountryData> GetCountryData();
        public abstract HashSet<RegionData> GetRegionData();
        public abstract HashSet<ProductData> GetProducts();

        public abstract HashSet<UserData> GetUserData();
    }
}