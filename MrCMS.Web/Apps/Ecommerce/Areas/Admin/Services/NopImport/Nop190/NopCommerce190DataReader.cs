using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
using StackExchange.Profiling;
using PaymentStatus = MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models.PaymentStatus;
using ShippingStatus = MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models.ShippingStatus;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Nop190
{
    public class NopCommerce190DataReader : NopCommerceDataReader
    {
        private const string FirstNameKey = "FirstName";
        private const string LastNameKey = "LastName";

        public override string Name
        {
            get { return "NopCommerce 1.90"; }
        }

        public override HashSet<PictureData> GetPictureData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Picture> pictures = context.Nop190_Pictures.ToHashSet();

                var usedPictureList = context.Nop190_ProductPictures.Select(x => x.PictureID).ToHashSet();

                pictures = pictures.FindAll(x => usedPictureList.Contains(x.PictureID));

                return pictures.Select(picture => new PictureData
                {
                    Id = picture.PictureID,
                    ContentType = picture.MimeType,
                    FileName = GetFileName(picture),
                    GetData = () => GetData(picture, PictureInfo)
                }).ToHashSet();
            }
        }

        private Stream GetData(Nop190_Picture picture, PictureInfo pictureInfo)
        {
            string fileName = picture.PictureID.ToString().PadLeft(7, '0') + "_0" + GetExtension(picture);
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

        private string GetFileName(Nop190_Picture picture)
        {
            string extension = GetExtension(picture);
            //string fileName = string.IsNullOrWhiteSpace(picture.SeoFilename)
            //    ? picture.Id.ToString()
            //    : picture.SeoFilename;
            //return string.Concat(fileName, extension);

            // GM: SeoFilename is new field
            string fileName = picture.PictureID.ToString();
            return string.Concat(fileName, extension);
        }

        private static string GetExtension(Nop190_Picture picture)
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
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Category> categories = context.Nop190_Categories.Where(x => !x.Deleted).ToHashSet();

                return categories.Select(category => new CategoryData
                {
                    Id = category.CategoryID,
                    Name = category.Name,
                    ParentId = category.ParentCategoryID == 0 ? (int?)null : category.ParentCategoryID,
                    Abstract = category.Description,
                    Published = category.Published,
                    Url = MrCMS.Web.Apps.Ecommerce.Helpers.NopImport.SeoHelper.GetSeoUrl("Category", category.CategoryID, category.SEName, category.Name),
                    //Url = urlRecords.ContainsKey(category.Id) ? urlRecords[category.Id].Slug : null, // GM: UrlRecords is new table
                    PictureId = category.PictureID
                }).ToHashSet();
            }
        }

        public override HashSet<ProductOptionData> GetProductOptions()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_ProductAttribute> productAttributes = context.Nop190_ProductAttributes.ToHashSet();

                return productAttributes.Select(attribute => new ProductOptionData
                {
                    Id = attribute.ProductAttributeID,
                    Name = attribute.Name,
                }).ToHashSet();
            }
        }

        public override HashSet<ProductOptionValueData> GetProductOptionValues()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_ProductVariantAttributeValue> values =
                    context.Nop190_ProductVariantAttributeValues.ToHashSet();

                return values.Select(value => new ProductOptionValueData
                {
                    Value = value.Name,
                    PriceAdjustment = value.PriceAdjustment,
                    WeightAdjustment = value.WeightAdjustment,
                    Id = value.ProductVariantAttributeValueID,
                    OptionId = value.Nop190_ProductVariant_ProductAttribute_Mapping.ProductAttributeID,
                    VariantId = value.Nop190_ProductVariant_ProductAttribute_Mapping.ProductVariantID,
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationData> GetProductSpecifications()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_SpecificationAttribute> attributes = context.Nop190_SpecificationAttributes.ToHashSet();

                return attributes.Select(attribute => new ProductSpecificationData
                {
                    Name = attribute.Name,
                    Id = attribute.SpecificationAttributeID
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationAttributeOptionData> GetProductSpecificationOptions()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_SpecificationAttributeOption> attributeOptions =
                    context.Nop190_SpecificationAttributeOptions.ToHashSet();
                return attributeOptions.Select(attribute => new ProductSpecificationAttributeOptionData
                {
                    ProductSpecificationId = attribute.SpecificationAttributeID,
                    Name = attribute.Name,
                    Id = attribute.SpecificationAttributeOptionID
                }).ToHashSet();
            }
        }

        public override HashSet<ProductSpecificationValueData> GetProductSpecificationValues()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Product_SpecificationAttribute_Mapping> specificationAttributeMappings =
                    context.Nop190_Product_SpecificationAttribute_Mappings.ToHashSet();
                return specificationAttributeMappings.Select(attribute => new ProductSpecificationValueData
                {
                    OptionId = attribute.SpecificationAttributeOptionID,
                    ProductId = attribute.ProductID,
                    DisplayOrder = attribute.DisplayOrder,
                    Id = attribute.ProductSpecificationAttributeID
                }).ToHashSet();
            }
        }

        public override HashSet<BrandData> GetBrands()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Manufacturer> manufacturers =
                    context.Nop190_Manufacturers.Where(x => !x.Deleted).ToHashSet();

                return manufacturers.Select(x => new BrandData
                {
                    Name = x.Name,
                    Id = x.ManufacturerID
                }).ToHashSet();
            }
        }

        public override HashSet<TagData> GetTags()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_ProductTag> productTags = context.Nop190_ProductTags.ToHashSet();

                return productTags.Select(tag => new TagData
                {
                    Name = tag.Name,
                    Id = tag.ProductTagID
                }).ToHashSet();
            }
        }

        public override HashSet<TaxData> GetTaxData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_TaxCategory> taxCategories = context.Nop190_TaxCategories.ToHashSet();

                return taxCategories.Select(taxCategory =>
                {
                    Nop190_TaxRate nop190TaxRate = taxCategory.Nop190_TaxRates.FirstOrDefault(x => x.TaxCategoryID == taxCategory.TaxCategoryID);
                    return new TaxData
                    {
                        Name = taxCategory.Name,
                        Rate = nop190TaxRate == null ? 0m : nop190TaxRate.Percentage,
                        Id = taxCategory.TaxCategoryID,
                        RegionId = nop190TaxRate == null ? (int?)null : nop190TaxRate.StateProvinceID
                    };
                }).ToHashSet();
            }
        }

        public override HashSet<CountryData> GetCountryData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Country> countries = context.Nop190_Countries.ToHashSet();

                return countries.Select(country => new CountryData
                {
                    Id = country.CountryID,
                    Name = country.Name,
                    IsoCode = country.ThreeLetterISOCode
                }).ToHashSet();
            }
        }

        public override HashSet<RegionData> GetRegionData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_StateProvince> list = context.Nop190_StateProvinces.ToHashSet();

                return list.Select(province => new RegionData
                {
                    Id = province.StateProvinceID,
                    Name = province.Name,
                    CountryId = province.CountryID
                }).ToHashSet();
            }
        }

        public override HashSet<ProductData> GetProducts()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Product> products = context.Nop190_Products.Where(x => !x.Deleted).ToHashSet();

                // GM: UrlRecord is new table
                //Dictionary<int, Nop190_UrlRecord> urlRecords =
                //    context.Nop190_UrlRecords.Where(x => x.EntityName == "Product")
                //        .ToHashSet().GroupBy(x => x.EntityId)
                //        .ToDictionary(x => x.Key, x => x.First());

                Dictionary<int, HashSet<Nop190_ProductVariant>> productVariants =
                    context.Nop190_ProductVariants.Where(x => !x.Deleted).ToHashSet()
                        .GroupBy(x => x.ProductID)
                        .ToDictionary(x => x.Key, x => x.ToHashSet());

                var brandIds =
                    context.Nop190_Product_Manufacturer_Mappings.GroupBy(x => x.ProductID)
                        .ToDictionary(x => x.Key, x => x.First().ProductManufacturerID);

                var productTags = context.Nop190_ProductTag_Product_Mappings.GroupBy(x => x.ProductID)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.ProductTagID).ToHashSet());
                var productCategories = context.Nop190_Product_Category_Mappings.GroupBy(x => x.ProductID)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.ProductCategoryID).ToHashSet());

                var productPictures = context.Nop190_ProductPictures.GroupBy(x => x.ProductID)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.PictureID).ToHashSet());

                var tierPriceDictionary = context.Nop190_TierPrices.ToHashSet().GroupBy(x => x.ProductVariantID)
                    .ToDictionary(x => x.Key, x => x.ToHashSet());

                return products.Select(product =>
                {
                    var id = product.ProductId;
                    int? brandId = brandIds.ContainsKey(id) ? brandIds[id] : (int?)null;
                    var productData = new ProductData
                    {
                        Name = product.Name,
                        Id = id,
                        Abstract = product.ShortDescription,
                        Description = product.FullDescription,
                        BrandId = brandId,
                        Tags = productTags.ContainsKey(id) ? productTags[id] : new HashSet<int>(),
                        Categories = productCategories.ContainsKey(id) ? productCategories[id] : new HashSet<int>(),
                        Pictures = productPictures.ContainsKey(id) ? productPictures[id] : new HashSet<int>(),
                        Published = product.Published,
                        Url = Ecommerce.Helpers.NopImport.SeoHelper.GetSeoUrl("Product", id, product.SEName, product.Name),
                    };
                    var variants = productVariants.ContainsKey(id) ? productVariants[id] : new HashSet<Nop190_ProductVariant>();
                    productData.ProductVariants =
                        variants
                            .Select(variant =>
                            {
                                Nop190_Download download = context.Nop190_Downloads.FirstOrDefault(x => x.DownloadID == variant.DownloadID);
                                var tierPrices = tierPriceDictionary.ContainsKey(variant.ProductVariantId)
                                    ? tierPriceDictionary[variant.ProductVariantId]
                                    : new HashSet<Nop190_TierPrice>();
                                var productVariantData = new ProductVariantData
                                {
                                    Name = variant.Name,
                                    Id = variant.ProductVariantId,
                                    BasePrice = variant.Price,
                                    PreviousPrice = variant.OldPrice,
                                    SKU = string.IsNullOrWhiteSpace(variant.SKU) ? variant.ProductVariantId.ToString() : variant.SKU,
                                    StockRemaining = variant.StockQuantity,
                                    TrackingPolicy =
                                        variant.ManageInventory == 0
                                            ? TrackingPolicy.DontTrack
                                            : TrackingPolicy.Track,
                                    TaxRate = variant.TaxCategoryID,
                                    Weight = variant.Weight,
                                    PartNumber = variant.ManufacturerPartNumber,
                                    //Gtin = variant.Gtin, // GM: Gtin is new field
                                    Download = variant.IsDownload,
                                    GiftCard = variant.IsGiftCard,
                                    GiftCardType =
                                        variant.GiftCardType == 0 ? GiftCardType.Virtual : GiftCardType.Physicial,
                                    RequiresShipping = variant.IsShipEnabled,
                                    MaxDownloads = variant.MaxNumberOfDownloads,
                                    DownloadDays = variant.DownloadExpirationDays,
                                    DownloadUrl =
                                        download == null ? null : download.DownloadURL,
                                    PriceBreaks = tierPrices.Select(price => new PriceBreakInfo
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
            using (MiniProfiler.Current.Step("Read order data from old system"))
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Order> orders =
                    context.Nop190_Orders.Where(order => !order.Deleted && order.OrderStatusID != 40).ToHashSet();
                Dictionary<int, HashSet<Nop190_OrderNote>> orderNotes =
                    context.Nop190_OrderNotes.ToHashSet()
                        .GroupBy(note => note.OrderID)
                        .ToDictionary(notes => notes.Key, notes => notes.ToHashSet());

                var countries = GetCountries(context);

                return orders.Select(order => new OrderData
                {
                    Id = order.OrderID,
                    Guid = order.OrderGUID,
                    OrderDate = order.CreatedOn,
                    //BillingAddressId = order.BillingAddressId, // GM: BillingAddressId is new field. Old is BillingAddress1,2 etc
                    //ShippingAddressId = order.ShippingAddressId, // GM: ShippingAddressId is new field. Old is ShippingAddress1,2 etc
                    CustomerId = order.CustomerID,
                    Email = order.BillingEmail,
                    OrderStatus = (OrderStatus)order.OrderStatusID,
                    PaymentStatus = (PaymentStatus)order.PaymentStatusID,
                    ShippingStatus = (ShippingStatus)order.ShippingStatusID,
                    OrderSubtotalInclTax = order.OrderSubtotalInclTax,
                    OrderSubtotalExclTax = order.OrderSubtotalExclTax,
                    OrderSubTotalDiscountInclTax = order.OrderSubTotalDiscountInclTax,
                    OrderSubTotalDiscountExclTax = order.OrderSubTotalDiscountExclTax,
                    OrderShippingInclTax = order.OrderShippingInclTax,
                    OrderShippingExclTax = order.OrderShippingExclTax,
                    OrderTax = order.OrderTax,
                    OrderDiscount = order.OrderDiscount,
                    OrderTotal = order.OrderTotal,
                    //RewardPointsWereAdded = order.RewardPointsWereAdded, // GM: RewardPointsWereAdded is new field
                    CustomerIp = order.CustomerIP,
                    Notes = orderNotes.ContainsKey(order.OrderID)
                        ? orderNotes[order.OrderID].Select(note => new OrderNoteData
                        {
                            Note = note.Note,
                            ShowToCustomer = note.DisplayToCustomer,
                            Date = note.CreatedOn
                        }).ToHashSet()
                        : new HashSet<OrderNoteData>(),
                    PaidDate = order.PaidDate,
                    ShippingMethodName = order.ShippingMethod,
                    PaymentMethod = order.PaymentMethodName,
                    BillingAddress = new AddressData
                    {
                        Address1 = order.BillingAddress1,
                        Address2 = order.BillingAddress2,
                        City = order.BillingCity,
                        Company = order.BillingCompany,
                        CountryCode = GetCountryCode(order.BillingCountryID, countries),
                        Email = order.BillingEmail,
                        FirstName = order.BillingFirstName,
                        LastName = order.BillingLastName,
                        PhoneNumber = order.BillingPhoneNumber,
                        PostalCode = order.BillingZipPostalCode,
                        StateProvince = order.BillingStateProvince
                    },
                    ShippingAddress = new AddressData
                    {
                        Address1 = order.ShippingAddress1,
                        Address2 = order.ShippingAddress2,
                        City = order.ShippingCity,
                        Company = order.ShippingCompany,
                        CountryCode = GetCountryCode(order.ShippingCountryID, countries),
                        Email = order.ShippingEmail,
                        FirstName = order.ShippingFirstName,
                        LastName = order.ShippingLastName,
                        PhoneNumber = order.ShippingPhoneNumber,
                        PostalCode = order.ShippingZipPostalCode,
                        StateProvince = order.ShippingStateProvince
                    }
                }).ToHashSet();
            }
        }

        public override HashSet<OrderLineData> GetOrderLineData()
        {
            using (MiniProfiler.Current.Step("Read order line data from old system"))
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_OrderProductVariant> orderProductVariants =
                    context.Nop190_OrderProductVariants.ToHashSet();
                var productVariants = context.Nop190_ProductVariants.ToHashSet()
                    .ToDictionary(x => x.ProductVariantId);
                var products = context.Nop190_Products.ToHashSet()
                    .ToDictionary(x => x.ProductId);
                return orderProductVariants.Select(line =>
                {
                    var variant = productVariants.ContainsKey(line.ProductVariantID)
                        ? productVariants[line.ProductVariantID]
                        : null;
                    var product = variant == null ? null : products.ContainsKey(variant.ProductID)
                        ? products[variant.ProductID]
                        : null;
                    return new OrderLineData
                    {
                        Id = line.OrderProductVariantID,
                        OrderId = line.OrderID,
                        Quantity = line.Quantity,
                        UnitPriceInclTax = line.UnitPriceInclTax,
                        UnitPriceExclTax = line.UnitPriceExclTax,
                        PriceInclTax = line.PriceInclTax,
                        PriceExclTax = line.PriceExclTax,
                        DiscountAmountInclTax = line.DiscountAmountInclTax,
                        DiscountAmountExclTax = line.DiscountAmountExclTax,
                        DownloadCount = line.DownloadCount,
                        // ItemWeight = line.ItemWeight, // GM: ItemWeight is new field
                        RequiresShipping = variant != null && variant.IsShipEnabled,
                        SKU = variant == null ? string.Empty :
                            string.IsNullOrWhiteSpace(variant.SKU)
                                ? variant.ProductVariantId.ToString()
                                : variant.SKU,
                        ProductName = variant == null ? null :
                            string.Format("{0} - {1}", product == null ? variant.Name : product.Name,
                                line.AttributeDescription)
                    };
                }).ToHashSet();
            }
        }

        public override HashSet<AddressData> GetAddressData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Address> addresses = context.Nop190_Addresses.ToHashSet();
                var stateProvinces = GetStateProvinces(context);
                var countries = GetCountries(context);
                return addresses.Select(address => GetAddressDataObject(address, stateProvinces, countries)).ToHashSet();
            }
        }

        private Dictionary<int, Nop190_Country> GetCountries(Nop190DataContext context)
        {
            return context.Nop190_Countries.ToList().ToDictionary(x => x.CountryID);
        }

        private static Dictionary<int, Nop190_StateProvince> GetStateProvinces(Nop190DataContext context)
        {
            return context.Nop190_StateProvinces.ToList().ToDictionary(x => x.StateProvinceID);
        }

        public override HashSet<UserData> GetUserData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Customer> customers =
                    context.Nop190_Customers.Where(x => !x.Deleted && x.Email != null && x.PasswordHash != null && !x.IsGuest).ToHashSet();

                HashSet<Nop190_CustomerAttribute> attributes =
                    context.Nop190_CustomerAttributes.ToHashSet();
                Dictionary<int, HashSet<Nop190_CustomerAttribute>> attributeUserDictionary = attributes.GroupBy(
                    x => x.CustomerId)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.ToHashSet());
                HashSet<Nop190_Address> addresses = context.Nop190_Addresses.ToHashSet();
                var stateProvinces = GetStateProvinces(context);
                var countries = GetCountries(context);

                var userDatas = new HashSet<UserData>();
                foreach (Nop190_Customer customer in customers)
                {
                    Nop190_Customer thisCustomer = customer;
                    Dictionary<string, string> customerAttributes =
                        attributeUserDictionary.ContainsKey(customer.CustomerID)
                            ? attributeUserDictionary[customer.CustomerID]
                                .GroupBy(attribute => attribute.Key)
                                .ToDictionary(grouping => grouping.Key, attribute => attribute.First().Value)
                            : new Dictionary<string, string>();
                    HashSet<Nop190_Address> customerAddresses =
                        addresses.FindAll(x => x.CustomerID == thisCustomer.CustomerID)
                            .ToHashSet();

                    userDatas.Add(new UserData
                    {
                        Id = thisCustomer.CustomerID,
                        Email = thisCustomer.Email,
                        Salt = thisCustomer.SaltKey,
                        Hash = thisCustomer.PasswordHash,
                        Active = thisCustomer.Active,
                        Format = "NopSHA1",
                        Guid = thisCustomer.CustomerGUID,
                        FirstName =
                            customerAttributes.ContainsKey(FirstNameKey)
                                ? customerAttributes[FirstNameKey]
                                : string.Empty,
                        LastName =
                            customerAttributes.ContainsKey(LastNameKey) ? customerAttributes[LastNameKey] : string.Empty,
                        AddressData = customerAddresses.Select(address => GetAddressDataObject(address, stateProvinces, countries)).ToHashSet()
                    });
                }
                return userDatas;
            }
        }

        private AddressData GetAddressDataObject(Nop190_Address address, Dictionary<int, Nop190_StateProvince> stateProvinces, Dictionary<int, Nop190_Country> countries)
        {
            Nop190_StateProvince state =
                stateProvinces.ContainsKey(address.StateProvinceID) ? stateProvinces[address.StateProvinceID] : null;
            return new AddressData
            {
                Id = address.AddressId,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Address1 = address.Address1,
                Address2 = address.Address2,
                Company = address.Company,
                City = address.City,
                StateProvince = state != null ? state.Name : string.Empty,
                PostalCode = address.ZipPostalCode,
                PhoneNumber = address.PhoneNumber,
                CountryCode = GetCountryCode(address.CountryID, countries),
                Email = address.Email
            };
        }

        private string GetCountryCode(int countryId, Dictionary<int, Nop190_Country> countries)
        {
            var country = countries.ContainsKey(countryId) ? countries[countryId] : null;
            return country == null ? null : country.TwoLetterISOCode;
        }


        private Nop190DataContext GetContext()
        {
            return new Nop190DataContext(ConnectionString);
        }
    }
}