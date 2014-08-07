using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseManager : IGoogleBaseManager
    {
        private readonly ISession _session;
        private readonly IProductVariantService _productVariantService;
        private readonly IOrderShippingService _orderShippingService;

        public GoogleBaseManager(ISession session,IProductVariantService productVariantService,
                                   IOrderShippingService orderShippingService)
        {
             _session = session;
            _productVariantService = productVariantService;
            _orderShippingService = orderShippingService;
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

        public IPagedList<GoogleBaseCategory> SearchGoogleBaseCategories(string queryTerm = null, int page = 1, int pageSize=10)
        {
            var categories = GoogleBaseTaxonomyData.GetCategories();

            return !string.IsNullOrWhiteSpace(queryTerm) ? categories.Where(x => x.Name.ToLower().Contains(queryTerm.ToLower()))
                .Paged(page, pageSize) : categories.Paged(page, pageSize);
        }

        /// <summary>
        /// Export Products To Google Base
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
            xml.WriteElementString("description", " Products from " + CurrentRequestData.CurrentSite.Name+" online store");

            var productVariants = _productVariantService.GetAllVariantsForGoogleBase();

            foreach (var pv in productVariants)
            {
                ExportGoogleBaseProduct(ref xml, pv, ns);
            }

            xml.WriteEndElement();
            xml.WriteEndElement();
            xml.WriteEndDocument();

            xml.Flush();
            var file = ms.ToArray();
            xml.Close();

            return file;
        }

        /// <summary>
        /// Export Google BaseProduct
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="pv"></param>
        /// <param name="ns"></param>
        private void ExportGoogleBaseProduct(ref XmlTextWriter xml, ProductVariant pv, string ns)
        {
            xml.WriteStartElement("item");

            //TITLE
            var title = String.Empty;
            if (!String.IsNullOrWhiteSpace(pv.DisplayName))
                title = pv.DisplayName;
            if (title.Length > 70)
                title = title.Substring(0, 70);
            xml.WriteElementString("title", title);

            //LINK
            if (pv.Product!=null && !String.IsNullOrWhiteSpace(pv.DisplayName))
                xml.WriteElementString("link", GeneralHelper.GetValidProductVariantUrl(pv));

            //DESCRIPTION
            xml.WriteStartElement("description");
            var description = String.Empty;
            if (pv.Product!=null && !String.IsNullOrWhiteSpace(pv.DisplayName))
                description = pv.Product.BodyContent;
            if (pv.Product != null && String.IsNullOrEmpty(description))
                description = pv.Product.Abstract;
            if (pv.Product != null && String.IsNullOrEmpty(description))
                description = pv.DisplayName;
            var descriptionBytes = Encoding.Default.GetBytes(description);
            description = Encoding.UTF8.GetString(descriptionBytes);
            xml.WriteCData(description);
            xml.WriteEndElement();

            var googleBaseProduct = pv.GoogleBaseProducts.FirstOrDefault();

            //CONDITION
            xml.WriteElementString("g", "condition", ns,
                                   googleBaseProduct != null
                                       ? googleBaseProduct.Condition.ToString()
                                       : ProductCondition.New.ToString());

            //PRICE
            xml.WriteElementString("g", "price", ns, pv.Price.ToCurrencyFormat());

            //AVAILABILITY
            var availability = "In Stock";
            if (pv.TrackingPolicy == TrackingPolicy.Track && pv.StockRemaining <= 0)
                availability = "Out of Stock";
            xml.WriteElementString("g", "availability", ns, availability);

            //GOOGLE PRODUCT CATEGORY
            if (googleBaseProduct != null)
                xml.WriteElementString("g", "google_product_category", ns, googleBaseProduct.Category);

            //PRODUCT CATEGORY
            if (pv.Product != null && pv.Product.Categories.Any() && !String.IsNullOrWhiteSpace(pv.Product.Categories.First().Name))
                xml.WriteElementString("g", "product_type", ns, pv.Product.Categories.First().Name);
            else
                if (googleBaseProduct != null)
                    xml.WriteElementString("g", "product_type", ns, googleBaseProduct.Category);

            //IMAGES
            if (pv.Product != null && pv.Product.Images.Any() && !String.IsNullOrWhiteSpace(pv.Product.Images.First().FileUrl))
            {
                xml.WriteElementString("g", "image_link", ns, GeneralHelper.GetValidImageUrl(pv.Product.Images.First().FileUrl));
            }
            if (pv.Product != null && pv.Product.Images.Count() > 1 && !String.IsNullOrWhiteSpace(pv.Product.Images.ToList()[1].FileUrl))
            {
                xml.WriteElementString("g", "additional_image_link", ns,GeneralHelper.GetValidImageUrl(pv.Product.Images.ToList()[1].FileUrl));
            }

            //BRAND
            if (pv.Product != null && pv.Product.Brand != null && !String.IsNullOrWhiteSpace(pv.Product.Brand.Name))
                xml.WriteElementString("g", "brand", ns, pv.Product.Brand.Name);

            //ID
            xml.WriteElementString("g", "id", ns, pv.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

            //GTIN
            if (!String.IsNullOrWhiteSpace(pv.Barcode))
                xml.WriteElementString("g", "gtin", ns, pv.Barcode);

            //MPN
            if (!String.IsNullOrWhiteSpace(pv.ManufacturerPartNumber))
                xml.WriteElementString("g", "mpn", ns, pv.ManufacturerPartNumber);

            if (googleBaseProduct != null)
            {
                //GENDER
                xml.WriteElementString("g", "gender", ns, googleBaseProduct.Gender.ToString());

                //AGE GROUP
                xml.WriteElementString("g", "age_group", ns, googleBaseProduct.AgeGroup.ToString());
            }

            //ITEM GROUP ID
            if (pv.Product != null)
                xml.WriteElementString("g", "item_group_id", ns,pv.Product.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

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
            SetGoogleBaseShipping(ref xml, pv, ns);

            //WEIGHT
            xml.WriteElementString("g", "shipping_weight", ns,
                                   string.Format(CultureInfo.InvariantCulture,
                                                 "{0} {1}", pv.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
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
        /// Set Google Base Shipping
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="ns"></param>
        /// <param name="xml"></param>
        private void SetGoogleBaseShipping(ref XmlTextWriter xml, ProductVariant pv, string ns)
        {
            var cart = new CartModel()
                {
                    Items = new List<CartItem>()
                        {
                            new CartItem()
                                {
                                    Quantity = 1,
                                    Item = pv
                                }
                        }
                };
            var shippingCalculations = _orderShippingService.GetCheapestShippingCalculationsForEveryCountry(cart);
            foreach (var shippingCalculation in shippingCalculations)
            {
                xml.WriteStartElement("g", "shipping", ns);
                if (shippingCalculation.Country != null && !String.IsNullOrWhiteSpace(shippingCalculation.Country.ISOTwoLetterCode))
                    xml.WriteElementString("g", "country", ns, shippingCalculation.Country.ISOTwoLetterCode);
                if (shippingCalculation.ShippingMethod != null && !String.IsNullOrWhiteSpace(shippingCalculation.ShippingMethod.Name))
                    xml.WriteElementString("g", "service", ns, shippingCalculation.ShippingMethod.Name);
                var price = shippingCalculation.GetPrice(cart);
                xml.WriteElementString("g", "price", ns,
                                       price != null
                                           ? price.Value.ToString(new CultureInfo("en-GB", false).NumberFormat)
                                           : 0.ToString(new CultureInfo("en-GB", false).NumberFormat));
                xml.WriteEndElement();
            }
        }
    }
}