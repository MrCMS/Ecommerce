using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Nop280
{
    public class NopCommerce280ProductReader : INopCommerceProductReader
    {
        public string Name
        {
            get { return "NopCommerce 2.80"; }
        }

        public List<CategoryData> GetCategoryData(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Category> categories = context.Categories.Where(x => !x.Deleted).ToList();

                return categories.Select(category => new CategoryData
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentCategoryId == 0 ? (int?) null : category.ParentCategoryId,
                    Abstract = category.Description,
                    Published = category.Published
                }).ToList();
            }
        }

        public List<ProductOptionData> GetProductOptions(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<ProductAttribute> productAttributes = context.ProductAttributes.ToList();

                return productAttributes.Select(attribute => new ProductOptionData
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                }).ToList();
            }
        }

        public List<ProductOptionValueData> GetProductOptionValues(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<ProductVariantAttributeValue> values = context.ProductVariantAttributeValues.ToList();

                return values.Select(value => new ProductOptionValueData
                {
                    Value = value.Name,
                    PriceAdjustment = value.PriceAdjustment,
                    WeightAdjustment = value.WeightAdjustment,
                    Id = value.Id,
                    OptionId = value.ProductVariant_ProductAttribute_Mapping.ProductAttributeId,
                    VariantId = value.ProductVariant_ProductAttribute_Mapping.ProductVariantId,
                }).ToList();
            }
        }

        public List<ProductSpecificationData> GetProductSpecifications(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<SpecificationAttribute> attributes = context.SpecificationAttributes.ToList();

                return attributes.Select(attribute => new ProductSpecificationData
                {
                    Name = attribute.Name,
                    Id = attribute.Id
                }).ToList();
            }
        }

        public List<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<SpecificationAttributeOption> attributeOptions =
                    context.SpecificationAttributeOptions.ToList();
                return attributeOptions.Select(attribute => new ProductSpecificationAttributeOptionData
                {
                    ProductSpecificationId = attribute.SpecificationAttributeId,
                    Name = attribute.Name,
                    Id = attribute.Id
                }).ToList();
            }
        }

        public List<ProductSpecificationValueData> GetProductSpecificationValues(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Product_SpecificationAttribute_Mapping> specificationAttributeMappings =
                    context.Product_SpecificationAttribute_Mappings.ToList();
                return specificationAttributeMappings.Select(attribute => new ProductSpecificationValueData
                {
                    OptionId = attribute.SpecificationAttributeOptionId,
                    ProductId = attribute.ProductId,
                    DisplayOrder = attribute.DisplayOrder,
                    Id = attribute.Id
                }).ToList();
            }
        }

        public List<BrandData> GetBrands(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Manufacturer> manufacturers = context.Manufacturers.Where(x => !x.Deleted).ToList();

                return manufacturers.Select(x => new BrandData
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToList();
            }
        }

        public List<TagData> GetTags(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<ProductTag> productTags = context.ProductTags.ToList();

                return productTags.Select(tag => new TagData
                {
                    Name = tag.Name,
                    Id = tag.Id
                }).ToList();
            }
        }

        public List<TaxData> GetTaxData(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<TaxCategory> taxCategories = context.TaxCategories.ToList();

                return taxCategories.Select(taxCategory => new TaxData
                {
                    Name = taxCategory.Name,
                    Rate = taxCategory.TaxRate == null ? 0m : taxCategory.TaxRate.Percentage,
                    Id = taxCategory.Id,
                    RegionId = taxCategory.TaxRate == null ? (int?) null : taxCategory.TaxRate.StateProvinceId
                }).ToList();
            }
        }

        public List<CountryData> GetCountryData(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Country> countries = context.Countries.ToList();

                return countries.Select(country => new CountryData
                {
                    Id = country.Id,
                    Name = country.Name,
                    IsoCode = country.TwoLetterIsoCode
                }).ToList();
            }
        }

        public List<RegionData> GetRegionData(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<StateProvince> list = context.StateProvinces.ToList();

                return list.Select(province => new RegionData
                {
                    Id = province.Id,
                    Name = province.Name,
                    CountryId = province.CountryId
                }).ToList();
            }
        }

        public List<ProductData> GetProducts(string connectionString)
        {
            using (Nop280DataContext context = GetContext(connectionString))
            {
                List<Product> products = context.Products.Where(x => !x.Deleted).ToList();

                return products.Select(product =>
                {
                    int? brandId =
                        product.Product_Manufacturer_Mappings.Select(mapping => (int?) mapping.ManufacturerId)
                            .FirstOrDefault();
                    var productData = new ProductData
                    {
                        Name = product.Name,
                        Id = product.Id,
                        Abstract = product.ShortDescription,
                        Description = product.FullDescription,
                        BrandId = brandId,
                        Tags = product.Product_ProductTag_Mappings.Select(mapping => mapping.ProductTag_Id).ToList(),
                        Categories = product.Product_Category_Mappings.Select(mapping => mapping.CategoryId).ToList(),
                        Published = product.Published
                    };
                    productData.ProductVariants =
                        context.ProductVariants.Where(x => !x.Deleted)
                            .Where(variant => variant.ProductId == product.Id)
                            .ToList()
                            .Select(variant =>
                            {
                                var productVariantData = new ProductVariantData
                                {
                                    Name = variant.Name,
                                    Id = variant.Id,
                                    BasePrice = variant.Price,
                                    PreviousPrice = variant.OldPrice,
                                    SKU = variant.Sku,
                                    StockRemaining = variant.StockQuantity,
                                    TrackingPolicy =
                                        variant.ManageInventoryMethodId == 0
                                            ? TrackingPolicy.DontTrack
                                            : TrackingPolicy.Track,
                                    TaxRate = variant.TaxCategoryId,
                                    Weight = variant.Weight,
                                    PartNumber = variant.ManufacturerPartNumber,
                                    Gtin = variant.Gtin,
                                    Download = variant.IsDownload,
                                    GiftCard = variant.IsGiftCard,
                                    GiftCardType =
                                        variant.GiftCardTypeId == 0 ? GiftCardType.Virtual : GiftCardType.Physicial,
                                    RequiresShipping = !variant.IsShipEnabled,
                                    MaxDownloads = variant.MaxNumberOfDownloads,
                                    DownloadDays = variant.DownloadExpirationDays,
                                    DownloadUrl = variant.Download == null ? null : variant.Download.DownloadUrl,
                                    PriceBreaks = variant.TierPrices.Select(price => new PriceBreakInfo
                                    {
                                        Price = price.Price,
                                        Quantity = price.Quantity
                                    }).ToList()
                                };

                                return productVariantData;
                            }).ToList();
                    return productData;
                }).ToList();
            }
        }


        private static Nop280DataContext GetContext(string connectionString)
        {
            return new Nop280DataContext(connectionString);
        }
    }
}