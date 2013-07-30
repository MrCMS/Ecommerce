using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        #region Props
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOrderShippingService _orderShippingService;
        #endregion

        #region Ctor
        public ImportExportManager(IImportProductsValidationService importProductsValidationService,
                                   IImportProductsService importProductsService,
                                   IProductVariantService productVariantService,
                                   IOrderShippingService orderShippingService)
        {
            _importProductsValidationService = importProductsValidationService;
            _importProductsService = importProductsService;
            _productVariantService = productVariantService;
            _orderShippingService = orderShippingService;
        }
        #endregion

        #region Products

        /// <summary>
        /// Import Products From Excel
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ImportProductsFromExcel(Stream file)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            var productsToImport = GetProductsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _importProductsValidationService.ValidateBusinessLogic(productsToImport);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            _importProductsService.ImportProductsFromDTOs(productsToImport);
            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private List<ProductImportDataTransferObject> GetProductsFromSpreadSheet(ExcelPackage spreadsheet,
                                                                                 out Dictionary<string, List<string>>
                                                                                     parseErrors)
        {
            parseErrors = _importProductsValidationService.ValidateImportFile(spreadsheet);

            return _importProductsValidationService.ValidateAndImportProductsWithVariants(spreadsheet, ref parseErrors);
        }

        /// <summary>
        /// Export Products To Excel
        /// </summary>
        /// <returns></returns>
        public byte[] ExportProductsToExcel()
        {
            using (var excelFile = new ExcelPackage())
            {
                var wsInfo = excelFile.Workbook.Worksheets.Add("Info");

                wsInfo.Cells["A1:D1"].Style.Font.Bold = true;
                wsInfo.Cells["A:D"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsInfo.Cells["A1"].Value = "MrCMS Version";
                wsInfo.Cells["B1"].Value = "Entity Type for Export";
                wsInfo.Cells["C1"].Value = "Export Date";
                wsInfo.Cells["D1"].Value = "Export Source";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Product";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DD hh:mm:ss";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["D2"].Value = "MrCMS " + MrCMSHtmlHelper.AssemblyVersion(null);

                wsInfo.Cells["A:D"].AutoFitColumns();
                wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";

                var wsItems = excelFile.Workbook.Worksheets.Add("Items");

                wsItems.Cells["A1:AE1"].Style.Font.Bold = true;
                wsItems.Cells["A:AE"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsItems.Cells["A1"].Value = "Url (Must not be changed!)";
                wsItems.Cells["B1"].Value = "Product Name";
                wsItems.Cells["C1"].Value = "Description";
                wsItems.Cells["D1"].Value = "SEO Title";
                wsItems.Cells["E1"].Value = "SEO Description";
                wsItems.Cells["F1"].Value = "SEO Keywords";
                wsItems.Cells["G1"].Value = "Abstract";
                wsItems.Cells["H1"].Value = "Brand";
                wsItems.Cells["I1"].Value = "Categories";
                wsItems.Cells["J1"].Value = "Specifications";
                wsItems.Cells["K1"].Value = "Variant Name";
                wsItems.Cells["L1"].Value = "Price";
                wsItems.Cells["M1"].Value = "Previous Price";
                wsItems.Cells["N1"].Value = "Tax Rate";
                wsItems.Cells["O1"].Value = "Weight (g)";
                wsItems.Cells["P1"].Value = "Stock";
                wsItems.Cells["Q1"].Value = "Tracking Policy";
                wsItems.Cells["R1"].Value = "SKU";
                wsItems.Cells["S1"].Value = "Barcode";
                wsItems.Cells["T1"].Value = "Option 1 Name";
                wsItems.Cells["U1"].Value = "Option 1 Value";
                wsItems.Cells["V1"].Value = "Option 2 Name";
                wsItems.Cells["W1"].Value = "Option 2 Value";
                wsItems.Cells["X1"].Value = "Option 3 Name";
                wsItems.Cells["Y1"].Value = "Option 3 Value";
                wsItems.Cells["Z1"].Value = "Image 1";
                wsItems.Cells["AA1"].Value = "Image 2";
                wsItems.Cells["AB1"].Value = "Image 3";
                wsItems.Cells["AC1"].Value = "Price Breaks";
                wsItems.Cells["AD1"].Value = "Url History";
                wsItems.Cells["AE1"].Value = "Publish Date";

                var productVariants = _productVariantService.GetAll();

                for (var i = 0; i < productVariants.Count; i++)
                {
                    var rowId = i + 2;
                    wsItems.Cells["A" + rowId].Value = productVariants[i].Product.UrlSegment;
                    wsItems.Cells["A" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["B" + rowId].Value = productVariants[i].Product.Name;
                    wsItems.Cells["B" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["C" + rowId].Value = productVariants[i].Product.BodyContent;
                    wsItems.Cells["C" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["D" + rowId].Value = productVariants[i].Product.MetaTitle;
                    wsItems.Cells["E" + rowId].Value = productVariants[i].Product.MetaDescription;
                    wsItems.Cells["E" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["F" + rowId].Value = productVariants[i].Product.MetaKeywords;
                    wsItems.Cells["G" + rowId].Value = productVariants[i].Product.Abstract;
                    wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    if (productVariants[i].Product.Brand != null)
                        wsItems.Cells["H" + rowId].Value = productVariants[i].Product.Brand.Name;
                    if (productVariants[i].Product.Categories.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Categories)
                        {
                            wsItems.Cells["I" + rowId].Value += item.UrlSegment + ";";
                        }
                    }
                    if (productVariants[i].Product.SpecificationValues.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.SpecificationValues)
                        {
                            wsItems.Cells["J" + rowId].Value += item.SpecificationName + ":" + item.Value + ";";
                        }
                        wsItems.Cells["J" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    wsItems.Cells["K" + rowId].Value = productVariants[i].Name ?? String.Empty;
                    wsItems.Cells["L" + rowId].Value = productVariants[i].BasePrice;
                    wsItems.Cells["M" + rowId].Value = productVariants[i].PreviousPrice;
                    if (productVariants[i].TaxRate != null)
                        wsItems.Cells["N" + rowId].Value = productVariants[i].TaxRate.Id;
                    wsItems.Cells["O" + rowId].Value = productVariants[i].Weight;
                    wsItems.Cells["P" + rowId].Value = productVariants[i].StockRemaining;
                    wsItems.Cells["Q" + rowId].Value = productVariants[i].TrackingPolicy;
                    wsItems.Cells["R" + rowId].Value = productVariants[i].SKU;
                    wsItems.Cells["S" + rowId].Value = productVariants[i].Barcode;

                    for (var v = 0;
                         v <
                         productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count();
                         v++)
                    {
                        if (v == 0)
                        {
                            wsItems.Cells["T" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["U" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 1)
                        {
                            wsItems.Cells["V" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["W" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 2)
                        {
                            wsItems.Cells["X" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["Y" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                    }

                    if (productVariants[i].PriceBreaks.Count > 0)
                    {
                        foreach (var item in productVariants[i].PriceBreaks)
                        {
                            wsItems.Cells["AC" + rowId].Value += item.Quantity + ":" + item.Price.ToString("#.##") + ";";
                        }
                        wsItems.Cells["AC" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Urls.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Urls)
                        {
                            wsItems.Cells["AD" + rowId].Value += item.UrlSegment + ",";
                        }
                        wsItems.Cells["AD" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    if (productVariants[i].Product.Published)
                        wsItems.Cells["AE" + rowId].Value = productVariants[i].Product.PublishOn;
                    wsItems.Cells["AE" + rowId].Style.Numberformat.Format = "YYYY-MM-DD hh:mm:ss";

                    if (!productVariants[i].Product.Images.Any()) continue;
                    wsItems.Cells["Z" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                       productVariants[i].Product.Images.First().FileUrl + "?update=no";
                    wsItems.Cells["Z" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    if (productVariants[i].Product.Images.Count() > 1)
                    {
                        wsItems.Cells["AA" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                            productVariants[i].Product.Images.ToList()[1].FileUrl +
                                                            "?update=no";
                        wsItems.Cells["AA" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Images.Count() > 2)
                    {
                        wsItems.Cells["AB" + rowId].Value = "http://" + CurrentRequestData.CurrentSite.BaseUrl +
                                                            productVariants[i].Product.Images.ToList()[2].FileUrl +
                                                            "?update=no";
                        wsItems.Cells["AB" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }
                wsItems.Cells["A:B"].AutoFitColumns();
                wsItems.Cells["D:D"].AutoFitColumns();
                wsItems.Cells["F:F"].AutoFitColumns();
                wsItems.Cells["I:AE"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }

        #endregion

        #region Google Base

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
                xml.WriteStartElement("item");

                //TITLE
                var title = pv.Name;
                if (title.Length > 70)
                    title = title.Substring(0, 70);
                xml.WriteElementString("title", title);

                //LINK
                xml.WriteElementString("link", string.Format("{0}?variant={1}", 
                    "http://" + CurrentRequestData.CurrentSite.BaseUrl + "/" + pv.Product.UrlSegment, pv.Id));

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
                xml.WriteElementString("g", "condition", ns, pv.GoogleBaseProduct.Condition.ToString());

                //PRICE
                xml.WriteElementString("g", "price", ns, pv.Price.ToCurrencyFormat());

                //AVAILABILITY
                var availability = "In Stock";
                if (pv.TrackingPolicy == TrackingPolicy.Track && pv.StockRemaining <= 0)
                    availability = "Out of Stock";
                xml.WriteElementString("g", "availability", ns, availability);

                //GOOGLE PRODUCT CATEGORY
                if (pv.GoogleBaseProduct != null &&
                    !String.IsNullOrWhiteSpace(pv.GoogleBaseProduct.OverrideCategory))
                    xml.WriteElementString("g", "google_product_category", ns, pv.GoogleBaseProduct.OverrideCategory);
                else
                    xml.WriteElementString("g", "google_product_category", ns, pv.GoogleBaseProduct.Category);

                //PRODUCT CATEGORY
                if (pv.Product.Categories.Any())
                    xml.WriteElementString("g", "product_type", ns, pv.Product.Categories.First().Name);

                //IMAGES
                if (pv.Product.Images.Any())
                {
                    xml.WriteElementString("g", "image_link", ns,
                                           "http://" + CurrentRequestData.CurrentSite.BaseUrl + pv.Product.Images.First().FileUrl);
                }
                if (pv.Product.Images.Count()>1)
                {
                    xml.WriteElementString("g", "additional_image_link", ns,
                                           "http://" + CurrentRequestData.CurrentSite.BaseUrl + pv.Product.Images.ToList()[1].FileUrl);
                }

                //BRAND
                if (pv.Product.Brand != null)
                    xml.WriteElementString("g", "brand", ns, pv.Product.Brand.Name);

                //ID
                xml.WriteElementString("g", "id", ns, pv.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

                //GTIN - SKU
                xml.WriteElementString("g", "gtin", ns, pv.SKU);

                //GENDER
                xml.WriteElementString("g", "gender", ns, pv.GoogleBaseProduct.Gender.ToString());

                //AGE GROUP
                xml.WriteElementString("g", "age_group", ns, pv.GoogleBaseProduct.AgeGroup.ToString());

                //ITEM GROUP ID
                xml.WriteElementString("g", "item_group_id", ns, pv.Product.Id.ToString(new CultureInfo("en-GB", false).NumberFormat));

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

                //SHIPPING
                SetGoogleBaseShipping(pv, ns, ref xml);

                //WEIGHT
                xml.WriteElementString("g", "shipping_weight", ns,
                                       string.Format(CultureInfo.InvariantCulture, 
                                       "{0} {1}",pv.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
                                       "kg"));

                //UNIT PRICING MEASURE
                xml.WriteElementString("g", "unit_pricing_measure", ns,
                                       string.Format(CultureInfo.InvariantCulture, "{0}{1}",
                                                     pv.Weight.ToString(new CultureInfo("en-GB", false).NumberFormat),
                                                     "kg"));

                //ADWORDS
                xml.WriteElementString("g", "adwords_grouping", ns, pv.GoogleBaseProduct.Grouping);
                xml.WriteElementString("g", "adwords_labels", ns, pv.GoogleBaseProduct.Labels);
                xml.WriteElementString("g", "adwords_redirect", ns, pv.GoogleBaseProduct.Redirect);

                xml.WriteEndElement();

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
        /// Set Google Base Shipping
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="ns"></param>
        /// <param name="xml"></param>
        private IEnumerable<ShippingCalculation> SetGoogleBaseShipping(ProductVariant pv, string ns, ref XmlTextWriter xml)
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
            return shippingCalculations;
        }

        #endregion
    }
}