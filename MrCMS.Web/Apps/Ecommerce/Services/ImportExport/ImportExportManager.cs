using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.ImportExport.DTOs;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Website;
using PdfSharp.Pdf;

namespace MrCMS.Web.Apps.Ecommerce.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportProductsValidationService _importProductsValidationService;
        private readonly IImportProductsService _importProductsService;
        private readonly IProductVariantService _productVariantService;
        private readonly IOrderShippingService _orderShippingService;

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

                wsItems.Cells["A1:AF1"].Style.Font.Bold = true;
                wsItems.Cells["A:AF"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                wsItems.Cells["O1"].Value = "Weight (kg)";
                wsItems.Cells["P1"].Value = "Stock";
                wsItems.Cells["Q1"].Value = "Tracking Policy";
                wsItems.Cells["R1"].Value = "SKU";
                wsItems.Cells["S1"].Value = "Barcode";
                wsItems.Cells["T1"].Value = "Manufacturer Part Number";
                wsItems.Cells["U1"].Value = "Option 1 Name";
                wsItems.Cells["V1"].Value = "Option 1 Value";
                wsItems.Cells["W1"].Value = "Option 2 Name";
                wsItems.Cells["X1"].Value = "Option 2 Value";
                wsItems.Cells["Y1"].Value = "Option 3 Name";
                wsItems.Cells["Z1"].Value = "Option 3 Value";
                wsItems.Cells["AA1"].Value = "Image 1";
                wsItems.Cells["AB1"].Value = "Image 2";
                wsItems.Cells["AC1"].Value = "Image 3";
                wsItems.Cells["AD1"].Value = "Price Breaks";
                wsItems.Cells["AE1"].Value = "Url History";
                wsItems.Cells["AF1"].Value = "Publish Date";

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
                    wsItems.Cells["T" + rowId].Value = productVariants[i].ManufacturerPartNumber;

                    for (var v = 0;
                         v <
                         productVariants[i].AttributeValues.OrderBy(x => x.ProductAttributeOption.DisplayOrder).Count();
                         v++)
                    {
                        if (v == 0)
                        {
                            wsItems.Cells["U" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["V" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 1)
                        {
                            wsItems.Cells["W" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["X" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                        if (v == 2)
                        {
                            wsItems.Cells["Y" + rowId].Value =
                                productVariants[i].AttributeValues[v].ProductAttributeOption.Name;
                            wsItems.Cells["Z" + rowId].Value = productVariants[i].AttributeValues[v].Value;
                        }
                    }

                    if (productVariants[i].PriceBreaks.Count > 0)
                    {
                        foreach (var item in productVariants[i].PriceBreaks)
                        {
                            wsItems.Cells["AD" + rowId].Value += item.Quantity + ":" + item.Price.ToString("#.##") + ";";
                        }
                        wsItems.Cells["AD" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Urls.Count > 0)
                    {
                        foreach (var item in productVariants[i].Product.Urls)
                        {
                            wsItems.Cells["AE" + rowId].Value += item.UrlSegment + ",";
                        }
                        wsItems.Cells["AE" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    if (productVariants[i].Product.Published)
                        wsItems.Cells["AF" + rowId].Value = productVariants[i].Product.PublishOn;
                    wsItems.Cells["AF" + rowId].Style.Numberformat.Format = "YYYY-MM-DD hh:mm:ss";

                    //Images
                    if (!productVariants[i].Product.Images.Any()) continue;

                    wsItems.Cells["AA" + rowId].Value = GenerateImageUrlForExport(productVariants[i].Product.Images.First().FileUrl);
                    wsItems.Cells["AA" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    if (productVariants[i].Product.Images.Count() > 1)
                    {
                        wsItems.Cells["AB" + rowId].Value = GenerateImageUrlForExport(productVariants[i].Product.Images.ToList()[1].FileUrl);
                        wsItems.Cells["AB" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    if (productVariants[i].Product.Images.Count() > 2)
                    {
                        wsItems.Cells["AC" + rowId].Value = GenerateImageUrlForExport(productVariants[i].Product.Images.ToList()[2].FileUrl);
                        wsItems.Cells["AC" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }
                wsItems.Cells["A:B"].AutoFitColumns();
                wsItems.Cells["D:D"].AutoFitColumns();
                wsItems.Cells["F:F"].AutoFitColumns();
                wsItems.Cells["I:AF"].AutoFitColumns();

                return excelFile.GetAsByteArray();
            }
        }

        private string GenerateImageUrlForExport(string imageUrl)
        {
            var siteUrl = "http://" + CurrentRequestData.CurrentSite.BaseUrl;

            return (!imageUrl.Contains("http") && !imageUrl.Contains("https"))
                                   ? (siteUrl + imageUrl + "?update=no")
                                   : imageUrl + "?update=no";
        }

        #endregion

        #region Orders

        public byte[] ExportOrderToPdf(Order order)
        {
            var pdf = SetDocumentInfo(order);

            SetDocumentStyles(ref pdf);

            SetDocument(ref pdf, order);

            return GetDocumentToByteArray(ref pdf);
        }

        private static Document SetDocumentInfo(Order order)
        {
            return new Document
            {
                Info =
                {
                    Title = CurrentRequestData.CurrentSite.Name + " Order: " + order.Guid,
                    Subject = CurrentRequestData.CurrentSite.Name + " Order: " + order.Guid,
                    Keywords = "MrCMS, Order",
                    Author = CurrentRequestData.CurrentUser.Name
                }
            };
        }

        private void SetDocumentStyles(ref Document document)
        {
            var style = document.Styles["Normal"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 10;

            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Tahoma";
            style.Font.Size = 9;
        }

        private void SetDocument(ref Document document, Order order)
        {
            var tableColor = new Color(0, 0, 0, 0);
            var section = document.AddSection();
            
            //HEADER
            SetHeader(ref section);

            //FOOTER
            SetFooter(ref section);

            //INFO
            SetInfo(order, ref section);

            //TABLE STYLE
            var table = SetTableStyle(ref section, tableColor);

            //HEADERS
            SetTableHeader(ref table, tableColor);

            //ITEMS
            SetTableData(order, ref table);

            //SUMMARY
            SetTableSummary(order, ref table);
        }

        private void SetHeader(ref Section section)
        {
            var frame1 = section.Headers.Primary.AddTextFrame();
            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Left;
            frame1.MarginTop = new Unit(1, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);

            var frame2 = section.Headers.Primary.AddTextFrame();
            frame2.RelativeVertical = RelativeVertical.Page;
            frame2.Left = ShapePosition.Right;
            frame2.MarginTop = new Unit(1, UnitType.Centimeter);
            frame2.Width = new Unit(2, UnitType.Centimeter);

            var p = frame1.AddParagraph();
            p.AddFormattedText(CurrentRequestData.CurrentSite.Name, TextFormat.Bold);
            p = frame2.AddParagraph();
            p.AddDateField("dd/MM/yyyy");
        }

        private void SetFooter(ref Section section)
        {
            var p = section.Footers.Primary.AddParagraph();
            p.Format.Alignment = ParagraphAlignment.Left;
            p.Format.Font.Size = 8;
            p.AddText(CurrentRequestData.CurrentSite.BaseUrl);
        }

        private void SetInfo(Order order, ref Section section)
        {
            var frame1 = section.AddTextFrame();
            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Left;
            frame1.Top = new Unit(1.85, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);
            var p = frame1.AddParagraph();
            p.Format.Font.Size = 16;
            p.AddFormattedText("Order #" + order.Id, TextFormat.Bold);

            //LEFT
            frame1 = section.AddTextFrame();
            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Left;
            frame1.Top = new Unit(3, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);

            //RIGHT
            var frame2 = section.AddTextFrame();
            frame2.RelativeVertical = RelativeVertical.Page;
            frame2.Left = ShapePosition.Right;
            frame2.Top = new Unit(3, UnitType.Centimeter);
            frame2.Width = new Unit(8, UnitType.Centimeter);

            //BILLING AND SHIPPING
            p = frame1.AddParagraph();
            p.AddFormattedText("Bill to:", TextFormat.Bold);
            p = frame2.AddParagraph();
            p.AddFormattedText("Ship to:", TextFormat.Bold);
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.Name);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.Name);
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.PhoneNumber);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.PhoneNumber);
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.Address1);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.Address1);
            if (!String.IsNullOrWhiteSpace(order.BillingAddress.Address2))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.Address2);
            }
            if (!String.IsNullOrWhiteSpace(order.ShippingAddress.Address2))
            {
                p = frame2.AddParagraph();
                p.AddText(order.ShippingAddress.Address2);
            }
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.City);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.City);
            if (!String.IsNullOrWhiteSpace(order.BillingAddress.StateProvince))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.StateProvince);
            }
            if (!String.IsNullOrWhiteSpace(order.ShippingAddress.StateProvince))
            {
                p = frame2.AddParagraph();
                p.AddText(order.ShippingAddress.StateProvince);
            }
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.Country.Name);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.Country.Name);
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.PostalCode);
            p = frame2.AddParagraph();
            p.AddText(order.ShippingAddress.PostalCode);

            frame1.AddParagraph("").AddLineBreak();
            frame2.AddParagraph("").AddLineBreak();

            //PAYMENT AND SHIPPING METHODS
            p = frame1.AddParagraph();
            p.AddText("Payment method: " + order.PaymentMethod);
            p = frame2.AddParagraph();
            p.AddText("Shipping method: " + order.ShippingMethod.Name);
        }

        private Table SetTableStyle(ref Section section, Color tableColor)
        {
            var frame = section.AddTextFrame();
            frame.MarginTop = new Unit(6, UnitType.Centimeter);
            frame.Width = new Unit(16, UnitType.Centimeter);

            //TABLE LABEL
            var p = frame.AddParagraph();
            p.AddFormattedText("Purchased goods:", TextFormat.Bold);

            frame.AddParagraph("").AddLineBreak();

            //TABLE
            var table = frame.AddTable();
            table.Style = "Table";
            table.Borders.Color = tableColor;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;
            return table;
        }

        private void SetTableHeader(ref Table table, Color tableColor)
        {
            var columns = new Dictionary<string, Dictionary<string, ParagraphAlignment>>()
                {
                    {
                        "#", new Dictionary<string, ParagraphAlignment>()
                            {
                                {"1cm", ParagraphAlignment.Center}
                            }
                    },
                    {
                        "Title", new Dictionary<string, ParagraphAlignment>()
                            {
                                {"6cm", ParagraphAlignment.Left}
                            }
                    },
                    {
                        "Unit Price", new Dictionary<string, ParagraphAlignment>()
                            {
                                {"3cm", ParagraphAlignment.Right}
                            }
                    },
                    {
                        "Qty", new Dictionary<string, ParagraphAlignment>()
                            {
                                {"3cm", ParagraphAlignment.Center}
                            }
                    },
                    {
                        "Total", new Dictionary<string, ParagraphAlignment>()
                            {
                                {"3cm", ParagraphAlignment.Right}
                            }
                    },
                };

            foreach (var item in columns)
            {
                var column = table.AddColumn(item.Value.First().Key);
                column.Format.Alignment = item.Value.First().Value;
            }

            var row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = tableColor;
            row.TopPadding = 2;
            row.BottomPadding = 2;
            var rowId = 0;
            foreach (var item in columns)
            {
                row.Cells[rowId].AddParagraph(item.Key);
                row.Cells[rowId].Format.Alignment = ParagraphAlignment.Center;
                rowId++;
            }

            table.SetEdge(0, 0, 5, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        private void SetTableData(Order order, ref Table table)
        {
            for (var i = 0; i < order.OrderLines.Count; i++)
            {
                var orderLine = order.OrderLines[i];
                var row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;

                row.Cells[0].AddParagraph((i + 1).ToString());
                row.Cells[1].AddParagraph(orderLine.ProductVariant.DisplayName);
                row.Cells[2].AddParagraph(orderLine.UnitPrice.ToCurrencyFormat());
                row.Cells[3].AddParagraph(orderLine.Quantity.ToString());
                row.Cells[4].AddParagraph(orderLine.Price.ToCurrencyFormat());

                table.SetEdge(0, table.Rows.Count - 2, 5, 2, Edge.Box, BorderStyle.Single, 0.75);
            }
        }

        private void SetTableSummary(Order order, ref Table table)
        {
            var summaryData = new Dictionary<string, string>()
                {
                    {"Sub-total", order.Subtotal.ToCurrencyFormat()},
                    {"Shipping", order.ShippingTotal.ToCurrencyFormat()},
                    {"Tax", order.Tax.ToCurrencyFormat()},
                    {"Discount", order.DiscountAmount.ToCurrencyFormat()},
                    {"Total", order.Total.ToCurrencyFormat()},
                };

            foreach (var item in summaryData)
            {
                var row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;
                row.Cells[0].Borders.Visible = false;
                row.Cells[0].AddParagraph(item.Key + ":");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[0].MergeRight = 3;
                if (item.Key == "Total")
                    row.Cells[4].Format.Font.Bold = true;
                row.Cells[4].AddParagraph(item.Value);
            }

            table.SetEdge(4, table.Rows.Count - 3, 1, 3, Edge.Box, BorderStyle.Single, 0.75);
        }

        private byte[] GetDocumentToByteArray(ref Document pdf)
        {
            var renderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Automatic) { Document = pdf };
            renderer.RenderDocument();
            var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream);
            return stream.ToArray();
        }

        #endregion
    }
}