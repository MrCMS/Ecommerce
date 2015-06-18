using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Models;
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

                HashSet<Nop190_ProductVariant> productVariants =
                    context.Nop190_ProductVariants.Where(x => !x.Deleted).ToHashSet();

                return products.Select(product =>
                {
                    int? brandId =
                        product.Nop190_Product_Manufacturer_Mappings.Select(mapping => (int?)mapping.ManufacturerID)
                            .FirstOrDefault();
                    var productData = new ProductData
                    {
                        Name = product.Name,
                        Id = product.ProductId,
                        Abstract = product.ShortDescription,
                        Description = product.FullDescription,
                        BrandId = brandId,
                        Tags =
                            product.Nop190_ProductTag_Product_Mappings.Select(mapping => mapping.ProductTagID)
                                .ToHashSet(),
                        Categories =
                            product.Nop190_Product_Category_Mappings.Select(mapping => mapping.CategoryID).ToHashSet(),
                        Pictures =
                            product.Nop190_ProductPictures.Select(mapping => mapping.PictureID).ToHashSet(),
                        Published = product.Published,
                        Url = MrCMS.Web.Apps.Ecommerce.Helpers.NopImport.SeoHelper.GetSeoUrl("Product", product.ProductId, product.SEName, product.Name),
                        //Url = urlRecords.ContainsKey(product.Id) ? urlRecords[product.Id].Slug : null // GM: UrlRecord is new table
                    };
                    productData.ProductVariants =
                        productVariants
                            .Where(variant => variant.ProductID == product.ProductId)
                            .Select(variant =>
                            {
                                Nop190_Download download = context.Nop190_Downloads.FirstOrDefault(x => x.DownloadID == variant.DownloadID);
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
                                    PriceBreaks = variant.Nop190_TierPrices.Select(price => new PriceBreakInfo
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
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Order> orders =
                    context.Nop190_Orders.Where(order => !order.Deleted && order.OrderStatusID != 40).ToHashSet();
                Dictionary<int, HashSet<Nop190_OrderNote>> orderNotes =
                    context.Nop190_OrderNotes.ToHashSet()
                        .GroupBy(note => note.OrderID)
                        .ToDictionary(notes => notes.Key, notes => notes.ToHashSet());
                Dictionary<int, Nop190_Address> addresses = context.Nop190_Addresses.ToDictionary(
                    address => address.AddressId, address => address);


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
                        Country = order.BillingCountryID,
                        Email = order.BillingEmail,
                        FirstName = order.BillingFirstName,
                        LastName= order.BillingLastName,
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
                        Country = order.ShippingCountryID,
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
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_OrderProductVariant> orderProductVariants =
                    context.Nop190_OrderProductVariants.ToHashSet();
                return orderProductVariants.Select(line => new OrderLineData
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
                    RequiresShipping = line.Nop190_ProductVariant.IsShipEnabled,
                    SKU =
                        string.IsNullOrWhiteSpace(line.Nop190_ProductVariant.SKU)
                            ? line.Nop190_ProductVariant.ProductVariantId.ToString()
                            : line.Nop190_ProductVariant.SKU,
                    ProductName =
                        string.Format("{0} - {1}", line.Nop190_ProductVariant.Nop190_Product.Name,
                            line.AttributeDescription)
                }).ToHashSet();
            }
        }

        public override HashSet<AddressData> GetAddressData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Address> addresses = context.Nop190_Addresses.ToHashSet();
                return addresses.Select(GetAddressDataObject).ToHashSet();
            }
        }

        public override HashSet<UserData> GetUserData()
        {
            using (Nop190DataContext context = GetContext())
            {
                HashSet<Nop190_Customer> customers =
                    context.Nop190_Customers.Where(x => !x.Deleted && x.Email != null && x.PasswordHash != null).ToHashSet();

                HashSet<Nop190_CustomerAttribute> attributes =
                    context.Nop190_CustomerAttributes.ToHashSet();
                Dictionary<int, HashSet<Nop190_CustomerAttribute>> attributeUserDictionary = attributes.GroupBy(
                    x => x.CustomerId)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.ToHashSet());
                HashSet<Nop190_Address> addresses = context.Nop190_Addresses.ToHashSet();

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
                        AddressData = customerAddresses.Select(GetAddressDataObject).ToHashSet()
                    });
                }
                return userDatas;
            }
        }

        private AddressData GetAddressDataObject(Nop190_Address address)
        {
            using (Nop190DataContext context = GetContext())
            {
                Nop190_StateProvince state = context.Nop190_StateProvinces.FirstOrDefault(x => x.StateProvinceID == address.StateProvinceID);
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
                    Country = address.CountryID,
                    Email = address.Email
                };
            }
        }


        private Nop190DataContext GetContext()
        {
            return new Nop190DataContext(ConnectionString);
        }
    }
}