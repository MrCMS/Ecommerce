using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Pricing;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseManager : IGoogleBaseManager
    {
        private readonly IGoogleBaseShippingService _googleBaseShippingService;
        private readonly IGetStockRemainingQuantity _getStockRemainingQuantity;
        private readonly IProductPricingMethod _productPricingMethod;
        private readonly IProductVariantService _productVariantService;
        private readonly ISession _session;

        public GoogleBaseManager(ISession session, IProductVariantService productVariantService,
            IGoogleBaseShippingService googleBaseShippingService, IGetStockRemainingQuantity getStockRemainingQuantity,
            IProductPricingMethod productPricingMethod)
        {
            _session = session;
            _productVariantService = productVariantService;
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
            _session.Transact(session => session.SaveOrUpdate(item));
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

            IList<ProductVariant> productVariants = _productVariantService.GetAllVariantsForGoogleBase();

            foreach (ProductVariant pv in productVariants)
            {
                ExportGoogleBaseProduct(ref xml, pv, ns);
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

            var sbOutput = new StringBuilder();

            foreach (var ch in inString.Where(ch => (ch >= 0x0020 && ch <= 0xD7FF) ||
                                                     (ch >= 0xE000 && ch <= 0xFFFD) ||
                                                     ch == 0x0009 ||
                                                     ch == 0x000A ||
                                                     ch == 0x000D))
            {
                sbOutput.Append(ch);
            }
            return sbOutput.ToString();
        }

        /// <summary>
        ///     Export Google BaseProduct
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="productVariant"></param>
        /// <param name="ns"></param>
        private void ExportGoogleBaseProduct(ref XmlTextWriter xml, ProductVariant productVariant, string ns)
        {
            xml.WriteStartElement("item");

            //TITLE
            string title = String.Empty;
            if (!String.IsNullOrWhiteSpace(productVariant.DisplayName))
                title = productVariant.DisplayName;
            if (title.Length > 70)
                title = title.Substring(0, 70);
            xml.WriteElementString("title", title);

            //LINK
            if (productVariant.Product != null && !String.IsNullOrWhiteSpace(productVariant.DisplayName))
                xml.WriteElementString("link", GeneralHelper.GetValidProductVariantUrl(productVariant));

            //DESCRIPTION
            xml.WriteStartElement("description");
            string description = String.Empty;
            if (productVariant.Product != null && !String.IsNullOrWhiteSpace(productVariant.DisplayName))
                description = productVariant.Product.BodyContent.StripHtml();
            if (productVariant.Product != null && String.IsNullOrEmpty(description))
                description = productVariant.Product.ProductAbstract.StripHtml();
            if (productVariant.Product != null && String.IsNullOrEmpty(description))
                description = productVariant.DisplayName.StripHtml();
            description = XmlCharacterWhitelist(description);
            byte[] descriptionBytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(descriptionBytes);
            xml.WriteCData(description);
            xml.WriteEndElement();

            GoogleBaseProduct googleBaseProduct = productVariant.GoogleBaseProducts.FirstOrDefault();

            //CONDITION
            xml.WriteElementString("g", "condition", ns,
                googleBaseProduct != null
                    ? googleBaseProduct.Condition.ToString()
                    : ProductCondition.New.ToString());

            //PRICE
            xml.WriteElementString("g", "price", ns, _productPricingMethod.GetUnitPrice(productVariant).ToCurrencyFormat());

            //AVAILABILITY
            string availability = "In Stock";
            if (productVariant.TrackingPolicy == TrackingPolicy.Track && _getStockRemainingQuantity.Get(productVariant) <= 0)
                availability = "Out of Stock";
            xml.WriteElementString("g", "availability", ns, availability);

            //GOOGLE PRODUCT CATEGORY
            if (googleBaseProduct != null)
                xml.WriteElementString("g", "google_product_category", ns, googleBaseProduct.Category);

            //PRODUCT CATEGORY
            if (productVariant.Product != null && productVariant.Product.Categories.Any() &&
                !String.IsNullOrWhiteSpace(productVariant.Product.Categories.First().Name))
                xml.WriteElementString("g", "product_type", ns, productVariant.Product.Categories.First().Name);
            else if (googleBaseProduct != null)
                xml.WriteElementString("g", "product_type", ns, googleBaseProduct.Category);

            //IMAGES
            if (productVariant.Product != null && productVariant.Product.Images.Any() &&
                !String.IsNullOrWhiteSpace(productVariant.Product.Images.First().FileUrl))
            {
                xml.WriteElementString("g", "image_link", ns,
                    GeneralHelper.GetValidImageUrl(productVariant.Product.Images.First().FileUrl));
            }
            if (productVariant.Product != null && productVariant.Product.Images.Count() > 1 &&
                !String.IsNullOrWhiteSpace(productVariant.Product.Images.ToList()[1].FileUrl))
            {
                xml.WriteElementString("g", "additional_image_link", ns,
                    GeneralHelper.GetValidImageUrl(productVariant.Product.Images.ToList()[1].FileUrl));
            }

            //BRAND
            if (productVariant.Product != null && productVariant.Product.BrandPage != null && !String.IsNullOrWhiteSpace(productVariant.Product.BrandPage.Name))
                xml.WriteElementString("g", "brand", ns, productVariant.Product.BrandPage.Name);

            //ID
            xml.WriteElementString("g", "id", ns, productVariant.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

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
            if (productVariant.Product != null)
                xml.WriteElementString("g", "item_group_id", ns,
                    productVariant.Product.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

            if (googleBaseProduct != null)
            {
                //COLOR
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Color))
                    xml.WriteElementString("g", "color", ns, googleBaseProduct.Color);

                //SIZE
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Size))
                    xml.WriteElementString("g", "size", ns, googleBaseProduct.Size);

                //PATTERN
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Pattern))
                    xml.WriteElementString("g", "pattern", ns, googleBaseProduct.Pattern);

                //MATERIAL
                if (!String.IsNullOrWhiteSpace(googleBaseProduct.Material))
                    xml.WriteElementString("g", "material", ns, googleBaseProduct.Material);
            }

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
                        Item = pv
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