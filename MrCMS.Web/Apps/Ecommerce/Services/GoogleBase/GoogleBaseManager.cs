using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;
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
                item.ProductVariant.GoogleBaseProduct = item;
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public IPagedList<GoogleBaseCategory> SearchGoogleBaseCategories(string queryTerm = null, int page = 1)
        {
            var categories = GoogleBaseTaxonomyData.GetCategories();

            return !string.IsNullOrWhiteSpace(queryTerm) ? categories.Where(x => x.Name.ToLower().Contains(queryTerm.ToLower())).Paged(page, 10) : categories.Paged(page, 10);
        }

        /// <summary>
        /// Export Products To Google Base
        /// </summary>
        /// <returns></returns>
        public byte[] ExportProductsToGoogleBase()
        {
            var ns = "http://base.google.com/ns/1.0";
            var ms = new MemoryStream();
            var xml = new XmlTextWriter(ms, Encoding.UTF8);

            xml.WriteStartDocument();
            xml.WriteStartElement("rss");
            xml.WriteAttributeString("xmlns", "g", null, ns);
            xml.WriteAttributeString("version", "2.0");
            xml.WriteStartElement("channel");

            //GENERAL FEED INFO
            xml.WriteElementString("title", CurrentRequestData.CurrentSite.Name);
            xml.WriteElementString("link", "http://"+CurrentRequestData.CurrentSite.BaseUrl);
            xml.WriteElementString("description", " Products from " + CurrentRequestData.CurrentSite.Name+" online store");

            var productVariants = _productVariantService.GetAllVariants(String.Empty);

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
            var title = pv.Name;
            if (title.Length > 70)
                title = title.Substring(0, 70);
            xml.WriteElementString("title", title);

            //LINK
            xml.WriteElementString("link", string.Format("http://{0}/{1}?variant={2}", CurrentRequestData.CurrentSite.BaseUrl, pv.Product.UrlSegment, pv.Id));

            //DESCRIPTION
            xml.WriteStartElement("description");
            var description = pv.Product.BodyContent;
            if (String.IsNullOrEmpty(description))
                description = pv.Product.Abstract;
            if (String.IsNullOrEmpty(description))
                description = pv.Name;
            if (String.IsNullOrEmpty(description))
                description = pv.Product.Name;
            xml.WriteCData(description);
            xml.WriteEndElement();

            //CONDITION
            if (pv.GoogleBaseProduct != null)
                xml.WriteElementString("g", "condition", ns, pv.GoogleBaseProduct.Condition.ToString());

            //PRICE
            xml.WriteElementString("g", "price", ns, pv.Price.ToCurrencyFormat());

            //AVAILABILITY
            var availability = "In Stock";
            if (pv.TrackingPolicy == TrackingPolicy.Track && pv.StockRemaining!=null && pv.StockRemaining <= 0)
                availability = "Out of Stock";
            xml.WriteElementString("g", "availability", ns, availability);

            //GOOGLE PRODUCT CATEGORY
            if (pv.GoogleBaseProduct != null)
            {
                if (pv.GoogleBaseProduct != null &&
                    !String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.OverrideCategory))
                    xml.WriteElementString("g", "google_product_category", ns, pv.GoogleBaseProduct.OverrideCategory);
                else
                    xml.WriteElementString("g", "google_product_category", ns, pv.GoogleBaseProduct.Category);
            }

            //PRODUCT CATEGORY
            if (pv.Product.Categories.Any())
                xml.WriteElementString("g", "product_type", ns, pv.Product.Categories.First().Name);

            //IMAGES
            if (pv.Product.Images.Any())
            {
                xml.WriteElementString("g", "image_link", ns,
                                       "http://" + CurrentRequestData.CurrentSite.BaseUrl + pv.Product.Images.First().FileUrl);
            }
            if (pv.Product.Images.Count() > 1)
            {
                xml.WriteElementString("g", "additional_image_link", ns,
                                       "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                       pv.Product.Images.ToList()[1].FileUrl);
            }

            //BRAND
            if (pv.Product.Brand != null)
                xml.WriteElementString("g", "brand", ns, pv.Product.Brand.Name);

            //ID
            xml.WriteElementString("g", "id", ns, pv.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

            //GTIN - SKU
            xml.WriteElementString("g", "gtin", ns, pv.SKU);

            if (pv.GoogleBaseProduct != null)
            {
                //GENDER
                xml.WriteElementString("g", "gender", ns, pv.GoogleBaseProduct.Gender.ToString());

                //AGE GROUP
                xml.WriteElementString("g", "age_group", ns, pv.GoogleBaseProduct.AgeGroup.ToString());
            }

            //ITEM GROUP ID
            xml.WriteElementString("g", "item_group_id", ns,
                                   pv.Product.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

            if (pv.GoogleBaseProduct != null)
            {
                //COLOR
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Color))
                    xml.WriteElementString("g", "color", ns, pv.GoogleBaseProduct.Color);

                //SIZE
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Size))
                    xml.WriteElementString("g", "size", ns, pv.GoogleBaseProduct.Size);

                //PATTERN
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Pattern))
                    xml.WriteElementString("g", "pattern", ns, pv.GoogleBaseProduct.Pattern);

                //MATERIAL
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Material))
                    xml.WriteElementString("g", "material", ns, pv.GoogleBaseProduct.Material);
            }

            //SHIPPING
            SetGoogleBaseShipping(ref xml, pv, ns);

            //WEIGHT
            xml.WriteElementString("g", "shipping_weight", ns,
                                   string.Format(CultureInfo.InvariantCulture,
                                                 "{0} {1}", pv.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
                                                 "kg"));

            //UNIT PRICING MEASURE
            xml.WriteElementString("g", "unit_pricing_measure", ns,
                                   string.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                 pv.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
                                                 "kg"));

            //ADWORDS
            if (pv.GoogleBaseProduct != null)
            {
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Grouping))
                    xml.WriteElementString("g", "adwords_grouping", ns, pv.GoogleBaseProduct.Grouping);
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Labels))
                    xml.WriteElementString("g", "adwords_labels", ns, pv.GoogleBaseProduct.Labels);
                if (!String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.Redirect))
                    xml.WriteElementString("g", "adwords_redirect", ns, pv.GoogleBaseProduct.Redirect);
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
                xml.WriteElementString("g", "country", ns, shippingCalculation.Country.Name);
                xml.WriteElementString("g", "service", ns, shippingCalculation.ShippingMethod.Name);
                xml.WriteElementString("g", "price", ns,shippingCalculation.GetPrice(cart).Value.ToString(new CultureInfo("en-GB", false).NumberFormat));
                xml.WriteEndElement();
            }
        }
    }
}