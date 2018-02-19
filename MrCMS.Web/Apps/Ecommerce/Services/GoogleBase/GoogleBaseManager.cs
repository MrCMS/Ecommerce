using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Website;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseManager : IGoogleBaseManager
    {
        private readonly IGoogleBaseShippingService _googleBaseShippingService;
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;
        private readonly ISession _session;
        private IProductPricingMethod _productPricingMethod;

        public GoogleBaseManager(ISession session,
            IGoogleBaseShippingService googleBaseShippingService, 
            IGetStockRemainingQuantity getStockRemainingQuantity, IProductPricingMethod productPricingMethod)
        {
            _session = session;
            _googleBaseShippingService = googleBaseShippingService;
            _getStockRemainingQuantity = getStockRemainingQuantity;
            _productPricingMethod = productPricingMethod;
        }


        public void SaveGoogleBaseProduct(GoogleBaseProduct item)
        {
            if (item.ProductVariant != null)
            {
                item.ProductVariant.GoogleBaseProducts.Clear();
                item.ProductVariant.GoogleBaseProducts.Add(item);
            }
            using (var transaction = _session.BeginTransaction())
            {
                if (item.Id == 0)
                    _session.Save(item);
                else
                    _session.Update(item);
                transaction.Commit();
            }
        }

        public IPagedList<GoogleBaseCategory> SearchGoogleBaseCategories(string queryTerm = null, int page = 1,
            int pageSize = 10)
        {
            IEnumerable<GoogleBaseCategory> categories = GoogleBaseTaxonomyData.GetCategories();

            return !string.IsNullOrWhiteSpace(queryTerm)
                ? categories.Where(x => x.Name.ToLower().Contains(queryTerm.ToLower()))
                    .Paged(page, pageSize)
                : categories.Paged(page, pageSize);
        }

        public class ProductCategoryMap
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
        }
        /// <summary>
        ///     Export Products To Google Base
        /// </summary>
        /// <returns></returns>
        public byte[] ExportProductsToGoogleBase()
        {
            const string ns = "http://base.google.com/ns/1.0";
            var ms = new MemoryStream();
            var xml = new XmlTextWriter(ms, Encoding.UTF8);

            xml.WriteStartDocument();
            xml.WriteStartElement("rss");
            xml.WriteAttributeString("xmlns", "g", null, ns);
            xml.WriteAttributeString("version", "2.0");
            xml.WriteStartElement("channel");

            //GENERAL FEED INFO
            xml.WriteElementString("title", CurrentRequestData.CurrentSite.Name);
            xml.WriteElementString("link", CurrentRequestData.CurrentSite.BaseUrl);
            xml.WriteElementString("description",
                " Products from " + CurrentRequestData.CurrentSite.Name + " online store");

            var productVariants = _session.QueryOver<ProductVariant>()
                .Fetch(x => x.Product).Eager
                .Fetch(x => x.TaxRate).Eager
                .Fetch(x => x.Product.BrandPage).Eager
                .Cacheable()
                .List().Where(x => x.Product != null && x.Product.Published).ToHashSet();
            var googleBaseProducts = _session.QueryOver<GoogleBaseProduct>().Cacheable().List()
                .GroupBy(x => x.ProductVariant.Id)
                .ToDictionary(x => x.Key, x => x.First());
            var optionValues = _session.QueryOver<ProductOptionValue>()
                .Fetch(x => x.ProductOption)
                .Eager.Cacheable()
                .List()
                .GroupBy(x => x.ProductVariant.Id)
                .ToDictionary(x => x.Key,
                    x => x.ToList());
            var categoryDictionary = _session.QueryOver<Category>().Cacheable().List().ToDictionary(x => x.Id);
            Product productAlias = null;
            ProductCategoryMap map = null;
            var productCategoryMaps = _session.QueryOver<Category>()
                .JoinAlias(x => x.Products, () => productAlias)
                .SelectList(builder =>
                {
                    builder.Select(x => x.Id).WithAlias(() => map.CategoryId);
                    builder.Select(() => productAlias.Id).WithAlias(() => map.ProductId);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<ProductCategoryMap>())
                .Cacheable()
                .List<ProductCategoryMap>().GroupBy(x => x.ProductId)
                .ToDictionary(x => x.Key,
                    x =>
                        x.Where(y => categoryDictionary.ContainsKey(y.CategoryId))
                            .Select(y => categoryDictionary[y.CategoryId])
                            .ToList());

            var files =
                _session.QueryOver<MediaFile>()
                    .Where(x => x.MediaCategory != null)
                    .List()
                    .Where(x => x.IsImage())
                    .GroupBy(x => x.MediaCategory.Id)
                    .ToDictionary(x => x.Key, x => x.OrderBy(mediaFile => mediaFile.DisplayOrder).ToList());
            var mediaCategoryFiles = _session.QueryOver<MediaCategory>().List().ToDictionary(x => x.Id, category =>
            {
                if (files.ContainsKey(category.Id))
                    return files[category.Id];
                return new List<MediaFile>();
            });
            var productImages =
                productVariants.Select(x => x.Product).Where(x => x != null).Distinct().ToDictionary(x => x.Id,
                    product =>
                    {
                        if (product.Gallery != null && mediaCategoryFiles.ContainsKey(product.Gallery.Id))
                        {
                            return mediaCategoryFiles[product.Gallery.Id];
                        }
                        return new List<MediaFile>();
                    });


            foreach (ProductVariant pv in productVariants)
            {
                var googleBaseProduct = googleBaseProducts.ContainsKey(pv.Id) ? googleBaseProducts[pv.Id] : null;
                var product = pv.Product;
                var mediaFiles = product == null ? new List<MediaFile>() : productImages[product.Id];
                if (!mediaFiles.Any())
                    continue;
                var options = optionValues.ContainsKey(pv.Id)
                    ? optionValues[pv.Id]
                    : new List<ProductOptionValue>();
                var categories = product == null || !productCategoryMaps.ContainsKey(product.Id)
                    ? new List<Category>()
                    : productCategoryMaps[product.Id];
                var brand = product == null ? null : product.BrandPage;
                ExportGoogleBaseProduct(ref xml, pv, ns, googleBaseProduct, options, categories, product, brand, mediaFiles);
            }

            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndDocument();

            xml.Flush();
            byte[] file = ms.ToArray();
            xml.Close();

            return file;
        }

        public static string XmlCharacterWhitelist(string inString)
        {
            if (inString == null) return null;

            StringBuilder sbOutput = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {
                ch = inString[i];
                if ((ch >= 0x0020 && ch <= 0xD7FF) ||
                    (ch >= 0xE000 && ch <= 0xFFFD) ||
                    ch == 0x0009 ||
                    ch == 0x000A ||
                    ch == 0x000D)
                {
                    sbOutput.Append(ch);
                }
            }
            return sbOutput.ToString();
        }

        /// <summary>
        ///     Export Google BaseProduct
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="productVariant"></param>
        /// <param name="ns"></param>
        /// <param name="googleBaseProduct"></param>
        /// <param name="options"></param>
        /// <param name="categories"></param>
        /// <param name="product"></param>
        /// <param name="brand"></param>
        /// <param name="images"></param>
        private void ExportGoogleBaseProduct(ref XmlTextWriter xml, ProductVariant productVariant, string ns, GoogleBaseProduct googleBaseProduct, List<ProductOptionValue> options, List<Category> categories, Product product, Brand brand, List<MediaFile> images)
        {
            xml.WriteStartElement("item");

            var optionValues = options.GroupBy(y => y.ProductOption.Name).ToDictionary(y => y.Key, y => y.First().Value,
                StringComparer.OrdinalIgnoreCase);

            //TITLE
            string title = String.Empty;
            var displayName = GetFullName(productVariant, options);
            if (!String.IsNullOrWhiteSpace(displayName))
                title = displayName;
            if (title.Length > 70)
                title = title.Substring(0, 70);
            xml.WriteElementString("title", title);

            //LINK
            if (product != null && !String.IsNullOrWhiteSpace(displayName))
                xml.WriteElementString("link", GeneralHelper.GetValidProductVariantUrl(productVariant));

            //DESCRIPTION
            xml.WriteStartElement("description");
            string description = String.Empty;
            if (product != null)
            {
                if (!String.IsNullOrWhiteSpace(displayName))
                    description = product.BodyContent.StripHtml().TruncateString(480);
                if (String.IsNullOrEmpty(description))
                    description = product.ProductAbstract.StripHtml().TruncateString(480);
                if (String.IsNullOrEmpty(description))
                    description = displayName.StripHtml().TruncateString(480);
            }
            
            description = XmlCharacterWhitelist(description);
            byte[] descriptionBytes = Encoding.Default.GetBytes(description.StripHtml());
            description = Encoding.UTF8.GetString(descriptionBytes);
            xml.WriteCData(description);
            xml.WriteEndElement();

            //CONDITION
            xml.WriteElementString("g", "condition", ns,
                googleBaseProduct != null
                    ? googleBaseProduct.Condition.ToString()
                    : ProductCondition.New.ToString());

            //PRICE
            xml.WriteElementString("g", "price", ns, _productPricingMethod.GetPrice(productVariant).ToCurrencyFormat());

            //AVAILABILITY
            string availability = "In Stock";
            if (productVariant.TrackingPolicy == TrackingPolicy.Track &&
                _getStockRemainingQuantity.Get(productVariant) <= 0)
                availability = "Out of Stock";
            xml.WriteElementString("g", "availability", ns, availability);

            //GOOGLE PRODUCT CATEGORY
            if (googleBaseProduct != null)
                xml.WriteElementString("g", "google_product_category", ns, googleBaseProduct.Category);

            //PRODUCT CATEGORY
            if (product != null && categories.Any() &&
                !String.IsNullOrWhiteSpace(categories.First().Name))
                xml.WriteElementString("g", "product_type", ns, categories.First().Name);
            else if (googleBaseProduct != null)
                xml.WriteElementString("g", "product_type", ns, googleBaseProduct.Category);

            //IMAGES
            if (product != null && images.Any() &&
                !String.IsNullOrWhiteSpace(images.First().FileUrl))
            {
                xml.WriteElementString("g", "image_link", ns,
                    GeneralHelper.GetValidImageUrl(images.First().FileUrl));
            }
            if (product != null && images.Count() > 1 &&
                !String.IsNullOrWhiteSpace(images.ToList()[1].FileUrl))
            {
                xml.WriteElementString("g", "additional_image_link", ns,
                    GeneralHelper.GetValidImageUrl(images.ToList()[1].FileUrl));
            }

            //BRAND
            if (brand != null &&
                !String.IsNullOrWhiteSpace(brand.Name))
                xml.WriteElementString("g", "brand", ns, brand.Name);

            //ID
            xml.WriteElementString("g", "id", ns,
                productVariant.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

            //GTIN
            if (!String.IsNullOrWhiteSpace(productVariant.Barcode))
                xml.WriteElementString("g", "gtin", ns, productVariant.Barcode);

            //MPN
            if (!String.IsNullOrWhiteSpace(productVariant.ManufacturerPartNumber))
                xml.WriteElementString("g", "mpn", ns, productVariant.ManufacturerPartNumber);

            if (googleBaseProduct != null)
            {
                //GENDER
                xml.WriteElementString("g", "gender", ns, googleBaseProduct.Gender.ToString());

                //AGE GROUP
                xml.WriteElementString("g", "age_group", ns, googleBaseProduct.AgeGroup.ToString());
            }

            //ITEM GROUP ID
            if (product != null)
                xml.WriteElementString("g", "item_group_id", ns,
                    product.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));
            //COLOR
            if (optionValues.ContainsKey("color"))
                xml.WriteElementString("g", "color", ns, optionValues["color"]);
            else if (optionValues.ContainsKey("colour"))
                xml.WriteElementString("g", "color", ns, optionValues["colour"]);

            //SIZE
            if (optionValues.ContainsKey("size"))
                xml.WriteElementString("g", "color", ns, optionValues["size"]);

            //PATTERN
            if (optionValues.ContainsKey("pattern"))
                xml.WriteElementString("g", "color", ns, optionValues["pattern"]);

            //MATERIAL
            if (optionValues.ContainsKey("material"))
                xml.WriteElementString("g", "color", ns, optionValues["material"]);

            //SHIPPING
            SetGoogleBaseShipping(ref xml, productVariant, ns);

            //WEIGHT
            xml.WriteElementString("g", "shipping_weight", ns,
                string.Format(CultureInfo.InvariantCulture,
                    "{0} {1}", productVariant.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
                    "kg"));

            //ADWORDS
            if (googleBaseProduct != null)
            {
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Grouping))
                    xml.WriteElementString("g", "adwords_grouping", ns, googleBaseProduct.Grouping);
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Labels))
                    xml.WriteElementString("g", "adwords_labels", ns, googleBaseProduct.Labels);
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Redirect))
                    xml.WriteElementString("g", "adwords_redirect", ns, googleBaseProduct.Redirect);
            }

            xml.WriteEndElement();
        }

        private string GetFullName(ProductVariant productVariant, List<ProductOptionValue> options)
        {
            var list = new List<string>();
            var product = productVariant.Product;
            if (product != null && !string.IsNullOrWhiteSpace(product.Name))
            {
                list.Add(product.Name);
            }
            var name = productVariant.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (product != null && product.Name != name)
                {
                    list.Add(name);
                }
            }
            if (options.Any())
            {
                list.AddRange(options.Select(option => option.FormattedValue));
            }
            return string.Join(" - ", list);
        }

        /// <summary>
        ///     Set Google Base Shipping
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="ns"></param>
        /// <param name="xml"></param>
        private void SetGoogleBaseShipping(ref XmlTextWriter xml, ProductVariant pv, string ns)
        {
            var cart = new CartModel
            {
                Items = new List<CartItemData>
                {
                    new CartItemData
                    {
                        Quantity = 1,
                        Item = pv,
                        Pricing = _productPricingMethod,
                    }
                }
            };
            IEnumerable<GoogleBaseCalculationInfo> shippingCalculations =
                _googleBaseShippingService.GetCheapestCalculationsForEachCountry(cart);
            foreach (GoogleBaseCalculationInfo shippingCalculation in shippingCalculations)
            {
                xml.WriteStartElement("g", "shipping", ns);
                xml.WriteElementString("g", "country", ns, shippingCalculation.CountryCode);
                xml.WriteElementString("g", "service", ns, shippingCalculation.ShippingMethodName);
                xml.WriteElementString("g", "price", ns,
                    shippingCalculation.Price.ToString(new CultureInfo("en-GB", false).NumberFormat));
                xml.WriteEndElement();
            }
        }
    }
}