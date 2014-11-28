using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Areas.Admin.Services.NopImport.Models;

namespace MrCMS.Web.Areas.Admin.Services.NopImport.Nop280
{
    public class NopCommerce280DataReader : NopCommerceDataReader
    {
        private const string FirstNameKey = "FirstName";
        private const string LastNameKey = "LastName";

        public override string Name
        {
            get { return "NopCommerce 2.80"; }
        }

        public override HashSet<CategoryData> GetCategoryData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Category> categories = context.Categories.Where(x => !x.Deleted).ToHashSet();
                var urlRecords =
                    context.UrlRecords.Where(x => x.EntityName == "Category")
                        .ToHashSet()
                        .GroupBy(x => x.EntityId).ToDictionary(x => x.Key, x => x.First());

                return categories.Select(category => new CategoryData
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentCategoryId == 0 ? (int?)null : category.ParentCategoryId,
                    Abstract = category.Description,
                    Published = category.Published,
                    Url = urlRecords.ContainsKey(category.Id) ? urlRecords[category.Id].Slug : null
                }).ToHashSet();
            }
        }

        public override HashSet<ProductOptionData> GetProductOptions()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<ProductAttribute> productAttributes = context.ProductAttributes.ToHashSet();

                return productAttributes.Select(attribute => new ProductOptionData
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                }).ToHashSet();
            }
        }

        public override HashSet<ProductOptionValueData> GetProductOptionValues()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<ProductVariantAttributeValue> values = context.ProductVariantAttributeValues.ToHashSet();

                return values.Select(value => new ProductOptionValueData
                {
                    Value = value.Name,
                    PriceAdjustment = value.PriceAdjustment,
                    WeightAdjustment = value.WeightAdjustment,
                    Id = value.Id,
                    OptionId = value.ProductVariant_ProductAttribute_Mapping.ProductAttributeId,
                    VariantId = value.ProductVariant_ProductAttribute_Mapping.ProductVariantId,
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationData> GetProductSpecifications()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<SpecificationAttribute> attributes = context.SpecificationAttributes.ToHashSet();

                return attributes.Select(attribute => new ProductSpecificationData
                {
                    Name = attribute.Name,
                    Id = attribute.Id
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<SpecificationAttributeOption> attributeOptions =
                    context.SpecificationAttributeOptions.ToHashSet();
                return attributeOptions.Select(attribute => new ProductSpecificationAttributeOptionData
                {
                    ProductSpecificationId = attribute.SpecificationAttributeId,
                    Name = attribute.Name,
                    Id = attribute.Id
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationValueData> GetProductSpecificationValues()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Product_SpecificationAttribute_Mapping> specificationAttributeMappings =
                    context.Product_SpecificationAttribute_Mappings.ToHashSet();
                return specificationAttributeMappings.Select(attribute => new ProductSpecificationValueData
                {
                    OptionId = attribute.SpecificationAttributeOptionId,
                    ProductId = attribute.ProductId,
                    DisplayOrder = attribute.DisplayOrder,
                    Id = attribute.Id
                }).ToHashSet();
            }
        }

        public override HashSet<BrandData> GetBrands()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Manufacturer> manufacturers = context.Manufacturers.Where(x => !x.Deleted).ToHashSet();

                return manufacturers.Select(x => new BrandData
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToHashSet();
            }
        }

        public override HashSet<TagData> GetTags()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<ProductTag> productTags = context.ProductTags.ToHashSet();

                return productTags.Select(tag => new TagData
                {
                    Name = tag.Name,
                    Id = tag.Id
                }).ToHashSet();
            }
        }

        public override HashSet<TaxData> GetTaxData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<TaxCategory> taxCategories = context.TaxCategories.ToHashSet();

                return taxCategories.Select(taxCategory => new TaxData
                {
                    Name = taxCategory.Name,
                    Rate = taxCategory.TaxRate == null ? 0m : taxCategory.TaxRate.Percentage,
                    Id = taxCategory.Id,
                    RegionId = taxCategory.TaxRate == null ? (int?)null : taxCategory.TaxRate.StateProvinceId
                }).ToHashSet();
            }
        }

        public override HashSet<CountryData> GetCountryData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Country> countries = context.Countries.ToHashSet();

                return countries.Select(country => new CountryData
                {
                    Id = country.Id,
                    Name = country.Name,
                    IsoCode = country.TwoLetterIsoCode
                }).ToHashSet();
            }
        }

        public override HashSet<RegionData> GetRegionData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<StateProvince> list = context.StateProvinces.ToHashSet();

                return list.Select(province => new RegionData
                {
                    Id = province.Id,
                    Name = province.Name,
                    CountryId = province.CountryId
                }).ToHashSet();
            }
        }

        public override HashSet<ProductData> GetProducts()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Product> products = context.Products.Where(x => !x.Deleted).ToHashSet();
                var urlRecords =
                    context.UrlRecords.Where(x => x.EntityName == "Product")
                        .ToHashSet().GroupBy(x => x.EntityId)
                        .ToDictionary(x => x.Key, x => x.First());

                return products.Select(product =>
                {
                    int? brandId = product.Product_Manufacturer_Mappings.Select(mapping => (int?)mapping.ManufacturerId) .FirstOrDefault();
                    var productData = new ProductData
                    {
                        Name = product.Name,
                        Id = product.Id,
                        Abstract = product.ShortDescription,
                        Description = product.FullDescription,
                        BrandId = brandId,
                        Tags = product.Product_ProductTag_Mappings.Select(mapping => mapping.ProductTag_Id).ToHashSet(),
                        Categories = product.Product_Category_Mappings.Select(mapping => mapping.CategoryId).ToHashSet(),
                        Published = product.Published,
                        Url= urlRecords.ContainsKey(product.Id) ? urlRecords[product.Id].Slug : null
                    };
                    productData.ProductVariants =
                        context.ProductVariants.Where(x => !x.Deleted)
                            .Where(variant => variant.ProductId == product.Id)
                            .ToHashSet()
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
                                    }).ToHashSet()
                                };

                                return productVariantData;
                            }).ToHashSet();
                    return productData;
                }).ToHashSet();
            }
        }

        public override HashSet<UserData> GetUserData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Customer> customers =
                    context.Customers.Where(x => !x.Deleted && x.Email != null && x.Password != null).ToHashSet();
                HashSet<GenericAttribute> attributes =
                    context.GenericAttributes.Where(attribute => attribute.KeyGroup == "Customer").ToHashSet();
                HashSet<CustomerAddress> addresses = context.CustomerAddresses.ToHashSet();

                var userDatas = new HashSet<UserData>();
                foreach (Customer customer in customers)
                {
                    Customer thisCustomer = customer;
                    Dictionary<string, string> customerAttributes =
                        attributes.FindAll(x => x.EntityId == thisCustomer.Id)
                            .GroupBy(attribute => attribute.Key)
                            .ToDictionary(grouping => grouping.Key, attribute => attribute.First().Value);
                    HashSet<Address> customerAddresses =
                        addresses.FindAll(x => x.Customer_Id == thisCustomer.Id)
                            .Select(address => address.Address)
                            .ToHashSet();

                    userDatas.Add(new UserData
                    {
                        Id = thisCustomer.Id,
                        Email = thisCustomer.Email,
                        Salt = thisCustomer.PasswordSalt,
                        Hash = thisCustomer.Password,
                        Active = thisCustomer.Active,
                        Format = "SHA1",
                        Guid = thisCustomer.CustomerGuid,
                        FirstName =
                            customerAttributes.ContainsKey(FirstNameKey)
                                ? customerAttributes[FirstNameKey]
                                : string.Empty,
                        LastName =
                            customerAttributes.ContainsKey(LastNameKey) ? customerAttributes[LastNameKey] : string.Empty,
                        AddressData = customerAddresses.Select(address => new AddressData
                        {
                            FirstName = address.FirstName,
                            LastName = address.LastName,
                            Address1 = address.Address1,
                            Address2 = address.Address2,
                            Company = address.Company,
                            City = address.City,
                            StateProvince = address.StateProvince != null ? address.StateProvince.Name : string.Empty,
                            PostalCode = address.ZipPostalCode,
                            PhoneNumber = address.PhoneNumber,
                            Country = address.CountryId,
                            Email = address.Email
                        }).ToHashSet()
                    });
                }
                return userDatas;
            }
        }


        private Nop280DataContext GetContext()
        {
            return new Nop280DataContext(ConnectionString);
        }
    }
}