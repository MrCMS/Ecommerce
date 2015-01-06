using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MrCMS.Helpers;
using MrCMS.Indexing.Management;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using PaymentStatus = MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models.PaymentStatus;
using ShippingStatus = MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models.ShippingStatus;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Nop280
{
    public class NopCommerce280DataReader : NopCommerceDataReader
    {
        private const string FirstNameKey = "FirstName";
        private const string LastNameKey = "LastName";

        public override string Name
        {
            get { return "NopCommerce 2.80"; }
        }

        public override HashSet<PictureData> GetPictureData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_Picture> pictures = context.Nop280_Pictures.ToHashSet();

                return pictures.Select(picture => new PictureData
                {
                    Id = picture.Id,
                    ContentType = picture.MimeType,
                    FileName = GetFileName(picture),
                    GetData = () => GetData(picture, PictureInfo)
                }).ToHashSet();
            }
        }

        private Stream GetData(Nop280_Picture picture, PictureInfo pictureInfo)
        {
            string fileName = picture.Id.ToString().PadLeft(7, '0') + "_0" + GetExtension(picture);
            switch (pictureInfo.PictureLocation)
            {
                case PictureLocation.OnDisc:
                    string discFolderLocation = GetOnDiscFolderLocation(pictureInfo.LocationData);
                    return File.OpenRead(discFolderLocation + fileName);
                case PictureLocation.Url:
                    string webFolder = GetWebFolderLocation(pictureInfo.LocationData);
                    return WebRequest.Create(webFolder + fileName).GetResponse().GetResponseStream();
                case PictureLocation.Database:
                    return new MemoryStream(picture.PictureBinary.ToArray());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetWebFolderLocation(string locationData)
        {
            return locationData.EndsWith("/") ? locationData : string.Concat(locationData, "/");
        }

        private string GetOnDiscFolderLocation(string locationData)
        {
            return locationData.EndsWith("\\") ? locationData : string.Concat(locationData, "\\");
        }

        private string GetFileName(Nop280_Picture picture)
        {
            string extension = GetExtension(picture);
            string fileName = string.IsNullOrWhiteSpace(picture.SeoFilename)
                ? picture.Id.ToString()
                : picture.SeoFilename;
            return string.Concat(fileName, extension);
        }

        private static string GetExtension(Nop280_Picture picture)
        {
            switch (picture.MimeType)
            {
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                case "image/pjpeg":
                    return ".jpg";
                default:
                    return ".jpeg";
            }
        }

        public override HashSet<CategoryData> GetCategoryData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_Category> categories = context.Nop280_Categories.Where(x => !x.Deleted).ToHashSet();
                Dictionary<int, Nop280_UrlRecord> urlRecords =
                    context.Nop280_UrlRecords.Where(x => x.EntityName == "Category")
                        .ToHashSet()
                        .GroupBy(x => x.EntityId).ToDictionary(x => x.Key, x => x.First());

                return categories.Select(category => new CategoryData
                {
                    Id = category.Id,
                    Name = category.Name,
                    ParentId = category.ParentCategoryId == 0 ? (int?)null : category.ParentCategoryId,
                    Abstract = category.Description,
                    Published = category.Published,
                    Url = urlRecords.ContainsKey(category.Id) ? urlRecords[category.Id].Slug : null,
                    PictureId = category.PictureId
                }).ToHashSet();
            }
        }

        public override HashSet<ProductOptionData> GetProductOptions()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_ProductAttribute> productAttributes = context.Nop280_ProductAttributes.ToHashSet();

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
                HashSet<Nop280_ProductVariantAttributeValue> values = context.Nop280_ProductVariantAttributeValues.ToHashSet();

                return values.Select(value => new ProductOptionValueData
                {
                    Value = value.Name,
                    PriceAdjustment = value.PriceAdjustment,
                    WeightAdjustment = value.WeightAdjustment,
                    Id = value.Id,
                    OptionId = value.Nop280_ProductVariant_ProductAttribute_Mapping.ProductAttributeId,
                    VariantId = value.Nop280_ProductVariant_ProductAttribute_Mapping.ProductVariantId,
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationData> GetProductSpecifications()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_SpecificationAttribute> attributes = context.Nop280_SpecificationAttributes.ToHashSet();

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
                HashSet<Nop280_SpecificationAttributeOption> attributeOptions =
                    context.Nop280_SpecificationAttributeOptions.ToHashSet();
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
                HashSet<Nop280_Product_SpecificationAttribute_Mapping> specificationAttributeMappings =
                    context.Nop280_Product_SpecificationAttribute_Mappings.ToHashSet();
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
                HashSet<Nop280_Manufacturer> manufacturers = context.Nop280_Manufacturers.Where(x => !x.Deleted).ToHashSet();

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
                HashSet<Nop280_ProductTag> productTags = context.Nop280_ProductTags.ToHashSet();

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
                HashSet<Nop280_TaxCategory> taxCategories = context.Nop280_TaxCategories.ToHashSet();

                return taxCategories.Select(taxCategory =>
                {
                    var nop280TaxRate = taxCategory.Nop280_TaxRate;
                    return new TaxData
                                                               {
                                                                   Name = taxCategory.Name,
                                                                   Rate = nop280TaxRate == null ? 0m : nop280TaxRate.Percentage,
                                                                   Id = taxCategory.Id,
                                                                   RegionId = nop280TaxRate == null ? (int?)null : nop280TaxRate.StateProvinceId
                                                               };
                }).ToHashSet();
            }
        }

        public override HashSet<CountryData> GetCountryData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_Country> countries = context.Nop280_Countries.ToHashSet();

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
                HashSet<Nop280_StateProvince> list = context.Nop280_StateProvinces.ToHashSet();

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
                HashSet<Nop280_Product> products = context.Nop280_Products.Where(x => !x.Deleted).ToHashSet();
                Dictionary<int, Nop280_UrlRecord> urlRecords =
                    context.Nop280_UrlRecords.Where(x => x.EntityName == "Product")
                        .ToHashSet().GroupBy(x => x.EntityId)
                        .ToDictionary(x => x.Key, x => x.First());
                var productVariants = context.Nop280_ProductVariants.Where(x => !x.Deleted).ToHashSet();

                return products.Select(product =>
                {
                    int? brandId =
                        product.Nop280_Product_Manufacturer_Mappings.Select(mapping => (int?)mapping.ManufacturerId)
                            .FirstOrDefault();
                    var productData = new ProductData
                    {
                        Name = product.Name,
                        Id = product.Id,
                        Abstract = product.ShortDescription,
                        Description = product.FullDescription,
                        BrandId = brandId,
                        Tags = product.Nop280_Product_ProductTag_Mappings.Select(mapping => mapping.ProductTag_Id).ToHashSet(),
                        Categories = product.Nop280_Product_Category_Mappings.Select(mapping => mapping.CategoryId).ToHashSet(),
                        Pictures = product.Nop280_Product_Picture_Mappings.Select(mapping => mapping.PictureId).ToHashSet(),
                        Published = product.Published,
                        Url = urlRecords.ContainsKey(product.Id) ? urlRecords[product.Id].Slug : null
                    };
                    productData.ProductVariants =
                       productVariants
                            .Where(variant => variant.ProductId == product.Id)

                            .Select(variant =>
                            {
                                var productVariantData = new ProductVariantData
                                {
                                    Name = variant.Name,
                                    Id = variant.Id,
                                    BasePrice = variant.Price,
                                    PreviousPrice = variant.OldPrice,
                                    SKU = string.IsNullOrWhiteSpace(variant.Sku) ? variant.Id.ToString() : variant.Sku,
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
                                    RequiresShipping = variant.IsShipEnabled,
                                    MaxDownloads = variant.MaxNumberOfDownloads,
                                    DownloadDays = variant.DownloadExpirationDays,
                                    DownloadUrl = variant.Nop280_Download == null ? null : variant.Nop280_Download.DownloadUrl,
                                    PriceBreaks = variant.Nop280_TierPrices.Select(price => new PriceBreakInfo
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

        public override HashSet<OrderData> GetOrderData()
        {
            using (var context = GetContext())
            {
                var orders = context.Nop280_Orders.Where(order => !order.Deleted && order.OrderStatusId != 40).ToHashSet();
                var orderNotes =
                    context.Nop280_OrderNotes.ToHashSet()
                        .GroupBy(note => note.OrderId)
                        .ToDictionary(notes => notes.Key, notes => notes.ToHashSet());
                var addresses = context.Nop280_Addresses.ToDictionary(address => address.Id, address => address);


                return orders.Select(order => new OrderData
                {
                    Id = order.Id,
                    Guid = order.OrderGuid,
                    OrderDate = order.CreatedOnUtc,
                    BillingAddressId = order.BillingAddressId,
                    ShippingAddressId = order.ShippingAddressId,
                    CustomerId = order.CustomerId,
                    Email = addresses[order.BillingAddressId].Email,
                    OrderStatus = (OrderStatus)order.OrderStatusId,
                    PaymentStatus = (PaymentStatus)order.PaymentStatusId,
                    ShippingStatus = (ShippingStatus)order.ShippingStatusId,
                    OrderSubtotalInclTax = order.OrderSubtotalInclTax,
                    OrderSubtotalExclTax = order.OrderSubtotalExclTax,
                    OrderSubTotalDiscountInclTax = order.OrderSubTotalDiscountInclTax,
                    OrderSubTotalDiscountExclTax = order.OrderSubTotalDiscountExclTax,
                    OrderShippingInclTax = order.OrderShippingInclTax,
                    OrderShippingExclTax = order.OrderShippingExclTax,
                    OrderTax = order.OrderTax,
                    OrderDiscount = order.OrderDiscount,
                    OrderTotal = order.OrderTotal,
                    RewardPointsWereAdded = order.RewardPointsWereAdded,
                    CustomerIp = order.CustomerIp,
                    Notes = orderNotes.ContainsKey(order.Id)
                        ? orderNotes[order.Id].Select(note => new OrderNoteData
                        {
                            Note = note.Note,
                            ShowToCustomer = note.DisplayToCustomer,
                            Date = note.CreatedOnUtc
                        }).ToHashSet()
                        : new HashSet<OrderNoteData>(),
                    PaidDate = order.PaidDateUtc,
                    ShippingMethodName = order.ShippingMethod,
                    PaymentMethod = order.PaymentMethodSystemName
                }).ToHashSet();
            }
        }

        public override HashSet<OrderLineData> GetOrderLineData()
        {
            using (var context = GetContext())
            {
                var orderProductVariants = context.Nop280_OrderProductVariants.ToHashSet();
                return orderProductVariants.Select(line => new OrderLineData
                {
                    Id = line.Id,
                    OrderId = line.OrderId,
                    Quantity = line.Quantity,
                    UnitPriceInclTax = line.UnitPriceInclTax,
                    UnitPriceExclTax = line.UnitPriceExclTax,
                    PriceInclTax = line.PriceInclTax,
                    PriceExclTax = line.PriceExclTax,
                    DiscountAmountInclTax = line.DiscountAmountInclTax,
                    DiscountAmountExclTax = line.DiscountAmountExclTax,
                    DownloadCount = line.DownloadCount,
                    ItemWeight = line.ItemWeight,
                    RequiresShipping = line.Nop280_ProductVariant.IsShipEnabled,
                    SKU = string.IsNullOrWhiteSpace(line.Nop280_ProductVariant.Sku) ? line.Nop280_ProductVariant.Id.ToString() : line.Nop280_ProductVariant.Sku,
                    ProductName = string.Format("{0} - {1}", line.Nop280_ProductVariant.Nop280_Product.Name, line.AttributeDescription)
                }).ToHashSet();
            }
        }

        public override HashSet<AddressData> GetAddressData()
        {
            using (var context = GetContext())
            {
                var addresses = context.Nop280_Addresses.ToHashSet();
                return addresses.Select(GetAddressDataObject).ToHashSet();
            }
        }

        public override HashSet<UserData> GetUserData()
        {
            using (Nop280DataContext context = GetContext())
            {
                HashSet<Nop280_Customer> customers =
                    context.Nop280_Customers.Where(x => !x.Deleted && x.Email != null && x.Password != null).ToHashSet();
                HashSet<Nop280_GenericAttribute> attributes =
                    context.Nop280_GenericAttributes.Where(attribute => attribute.KeyGroup == "Customer").ToHashSet();
                HashSet<Nop280_CustomerAddress> addresses = context.Nop280_CustomerAddresses.ToHashSet();

                var userDatas = new HashSet<UserData>();
                foreach (Nop280_Customer customer in customers)
                {
                    Nop280_Customer thisCustomer = customer;
                    Dictionary<string, string> customerAttributes =
                        attributes.FindAll(x => x.EntityId == thisCustomer.Id)
                            .GroupBy(attribute => attribute.Key)
                            .ToDictionary(grouping => grouping.Key, attribute => attribute.First().Value);
                    HashSet<Nop280_Address> customerAddresses =
                        addresses.FindAll(x => x.Customer_Id == thisCustomer.Id)
                            .Select(address => address.Nop280_Address)
                            .ToHashSet();

                    userDatas.Add(new UserData
                    {
                        Id = thisCustomer.Id,
                        Email = thisCustomer.Email,
                        Salt = thisCustomer.PasswordSalt,
                        Hash = thisCustomer.Password,
                        Active = thisCustomer.Active,
                        Format = "NopSHA1",
                        Guid = thisCustomer.CustomerGuid,
                        FirstName =
                            customerAttributes.ContainsKey(FirstNameKey)
                                ? customerAttributes[FirstNameKey]
                                : string.Empty,
                        LastName =
                            customerAttributes.ContainsKey(LastNameKey) ? customerAttributes[LastNameKey] : string.Empty,
                        AddressData = customerAddresses.Select(GetAddressDataObject).ToHashSet()
                    });
                }
                return userDatas;
            }
        }

        private static AddressData GetAddressDataObject(Nop280_Address address)
        {
            return new AddressData
            {
                Id = address.Id,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Address1 = address.Address1,
                Address2 = address.Address2,
                Company = address.Company,
                City = address.City,
                StateProvince = address.Nop280_StateProvince != null ? address.Nop280_StateProvince.Name : string.Empty,
                PostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                Country = address.CountryId,
                Email = address.Email
            };
        }


        private Nop280DataContext GetContext()
        {
            return new Nop280DataContext(ConnectionString);
        }
    }
}