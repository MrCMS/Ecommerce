using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport
{
    public abstract class NopCommerceDataReader : INopCommerceDataReader
    {
        protected string ConnectionString { get; set; }
        protected PictureInfo PictureInfo { get; set; }

        public abstract string Name { get; }

        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void SetPictureInfo(PictureInfo pictureInfo)
        {
            PictureInfo = pictureInfo;
        }

        public abstract HashSet<PictureData> GetPictureData();

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
        public abstract HashSet<OrderData> GetOrderData();
        public abstract HashSet<OrderLineData> GetOrderLineData();
        public abstract HashSet<AddressData> GetAddressData();
    }
}