using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Color = MigraDoc.DocumentObjectModel.Color;
using Image = System.Drawing.Image;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderInvoiceService : IOrderInvoiceService
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly IGetLogoUrl _getLogoUrl;
        private readonly TaxSettings _taxSettings;

        public OrderInvoiceService(EcommerceSettings ecommerceSettings, IGetLogoUrl getLogoUrl, TaxSettings taxSettings)
        {
            _ecommerceSettings = ecommerceSettings;
            _getLogoUrl = getLogoUrl;
            _taxSettings = taxSettings;
        }

        [MrCMSACLRule(typeof(ExportOrderACL), ExportOrderACL.ExportOrderToPdf)]
        public byte[] GeneratePDF(Order order)
        {
            Document pdf = SetDocumentInfo(order);

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
            Style style = document.Styles["Normal"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 10;

            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Tahoma";
            style.Font.Size = 9;
        }

        private void SetDocument(ref Document document, Order order)
        {
            var tableColor = new Color(0, 0, 0, 0);
            Section section = document.AddSection();

            //HEADER
            SetHeader(ref section);

            //FOOTER
            SetFooter(ref section);

            //INFO
            SetInfo(order, ref section);

            //TABLE STYLE
            Table table = SetTableStyle(ref section, tableColor);

            //HEADERS
            SetTableHeader(ref table, tableColor);

            //ITEMS
            SetTableData(order, ref table);

            //SUMMARY
            SetTableSummary(order, ref table);
        }

        private void SetHeader(ref Section section)
        {
            TextFrame frame1 = section.Headers.Primary.AddTextFrame();
            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Left;
            frame1.MarginTop = new Unit(1, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);

            TextFrame frame2 = section.Headers.Primary.AddTextFrame();
            frame2.RelativeVertical = RelativeVertical.Page;
            frame2.Left = ShapePosition.Right;
            frame2.MarginTop = new Unit(1, UnitType.Centimeter);
            frame2.Width = new Unit(2, UnitType.Centimeter);

            Paragraph p = frame1.AddParagraph();
            p.AddFormattedText(CurrentRequestData.CurrentSite.Name, TextFormat.Bold);
            p = frame2.AddParagraph();
            p.AddDateField("dd/MM/yyyy");
        }

        private void SetFooter(ref Section section)
        {
            if (!String.IsNullOrWhiteSpace(_ecommerceSettings.ReportFooterText))
            {
                Paragraph p = section.Footers.Primary.AddParagraph();
                p.Format.Alignment = ParagraphAlignment.Left;
                p.Format.Font.Size = 8;
                p.AddText(_ecommerceSettings.ReportFooterText);
            }
        }

        private void SetInfo(Order order, ref Section section)
        {
            TextFrame frame1 = section.AddTextFrame();
            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Right;
            frame1.Top = new Unit(1.85, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);
            Paragraph p = frame1.AddParagraph();
            p.Format.Font.Size = 16;
            p.Format.Alignment = ParagraphAlignment.Right;
            p.AddFormattedText("Order #" + order.Id, TextFormat.Bold);

            frame1 = section.AddTextFrame();
            TextFrame frame2 = section.AddTextFrame();

            if (!String.IsNullOrWhiteSpace(_ecommerceSettings.ReportLogoImage) &&
                !_ecommerceSettings.ReportLogoImage.Contains("http"))
            {
                string logoUrl = _getLogoUrl.Get();
                if (!String.IsNullOrWhiteSpace(logoUrl))
                {
                    MigraDoc.DocumentObjectModel.Shapes.Image logo = section.AddImage(logoUrl);
                    logo.RelativeVertical = RelativeVertical.Page;
                    logo.Left = ShapePosition.Left;
                    logo.Top = new Unit(1.85, UnitType.Centimeter);
                    logo.Height = new Unit(1.5, UnitType.Centimeter);
                    frame1.MarginTop = new Unit(0.5, UnitType.Centimeter);
                    frame2.MarginTop = new Unit(0.5, UnitType.Centimeter);
                }
            }

            frame1.RelativeVertical = RelativeVertical.Page;
            frame1.Left = ShapePosition.Left;
            frame1.Top = new Unit(3.5, UnitType.Centimeter);
            frame1.Width = new Unit(10, UnitType.Centimeter);

            frame2.RelativeVertical = RelativeVertical.Page;
            frame2.Left = ShapePosition.Right;
            frame2.Top = new Unit(3.5, UnitType.Centimeter);
            frame2.Width = new Unit(8, UnitType.Centimeter);

            //BILLING AND SHIPPING
            p = frame1.AddParagraph();
            p.AddFormattedText("Bill to:", TextFormat.Bold);
            p = frame2.AddParagraph();
            p.AddFormattedText("Ship to:", TextFormat.Bold);
            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.Name);
            if (!string.IsNullOrEmpty(order.BillingAddress.Company))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.Company);
            }

            if (order.ShippingAddress != null)
            {
                p = frame2.AddParagraph();
                p.AddText(order.ShippingAddress.Name);

                if (!string.IsNullOrEmpty(order.ShippingAddress.Company))
                {
                    p = frame2.AddParagraph();
                    p.AddText(order.ShippingAddress.Company);
                }
            }

            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.PhoneNumber);

            if (order.ShippingAddress != null)
            {
                p = frame2.AddParagraph();
                p.AddText(order.ShippingAddress.PhoneNumber);
            }

            p = frame1.AddParagraph();
            p.AddText(order.BillingAddress.Address1);

            if (order.ShippingAddress != null)
            {
                p = frame2.AddParagraph();
                p.AddText(order.ShippingAddress.Address1);
            }

            if (!String.IsNullOrWhiteSpace(order.BillingAddress.Address2))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.Address2);
            }

            if (order.ShippingAddress != null)
            {
                if (!String.IsNullOrWhiteSpace(order.ShippingAddress.Address2))
                {
                    p = frame2.AddParagraph();
                    p.AddText(order.ShippingAddress.Address2);
                }
            }

            if (!String.IsNullOrWhiteSpace(order.BillingAddress.City))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.City);
            }

            if (order.ShippingAddress != null)
            {
                if (!String.IsNullOrWhiteSpace(order.ShippingAddress.City))
                {
                    p = frame2.AddParagraph();
                    p.AddText(order.ShippingAddress.City);
                }
            }

            if (!String.IsNullOrWhiteSpace(order.BillingAddress.StateProvince))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.StateProvince);
            }

            if (order.ShippingAddress != null)
            {
                if (!String.IsNullOrWhiteSpace(order.ShippingAddress.StateProvince))
                {
                    p = frame2.AddParagraph();
                    p.AddText(order.ShippingAddress.StateProvince);
                }
            }

            string billingCountryName = order.BillingAddress.GetCountryName();
            if (!string.IsNullOrWhiteSpace(billingCountryName))
            {
                p = frame1.AddParagraph();
                p.AddText(billingCountryName);
            }

            if (order.ShippingAddress != null)
            {
                string shippingCountryName = order.ShippingAddress.GetCountryName();
                if (!string.IsNullOrWhiteSpace(shippingCountryName))
                {
                    p = frame2.AddParagraph();
                    p.AddText(shippingCountryName);
                }
            }

            if (!String.IsNullOrWhiteSpace(order.BillingAddress.PostalCode))
            {
                p = frame1.AddParagraph();
                p.AddText(order.BillingAddress.PostalCode);
            }

            if (order.ShippingAddress != null)
            {
                if (!String.IsNullOrWhiteSpace(order.ShippingAddress.PostalCode))
                {
                    p = frame2.AddParagraph();
                    p.AddText(order.ShippingAddress.PostalCode);
                }
            }

            frame1.AddParagraph("").AddLineBreak();
            frame2.AddParagraph("").AddLineBreak();

            //PAYMENT AND SHIPPING METHODS
            p = frame1.AddParagraph();
            p.AddText(string.Format("Payment method: {0}", order.PaymentMethod.BreakUpString()));
            p = frame2.AddParagraph();
            p.AddText(string.Format("Shipping method: {0}", order.ShippingMethodName));
        }

        private Table SetTableStyle(ref Section section, Color tableColor)
        {
            TextFrame frame = section.AddTextFrame();
            frame.MarginTop = new Unit(6, UnitType.Centimeter);
            frame.Width = new Unit(16, UnitType.Centimeter);

            //TABLE LABEL
            Paragraph p = frame.AddParagraph();
            p.AddFormattedText("Purchased goods:", TextFormat.Bold);

            frame.AddParagraph("").AddLineBreak();

            //TABLE
            Table table = frame.AddTable();
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
            var columns = new Dictionary<string, Dictionary<string, ParagraphAlignment>>();
            columns.Add("#", new Dictionary<string, ParagraphAlignment>
            {
                {"1cm", ParagraphAlignment.Center}
            });
            columns.Add("Title", new Dictionary<string, ParagraphAlignment>
            {
                {"6cm", ParagraphAlignment.Left}
            });
            var taxesEnabled = _taxSettings.TaxesEnabled;
            var columnWidth = taxesEnabled ? "2cm" : "3cm";
            columns.Add("Qty", new Dictionary<string, ParagraphAlignment>
            {
                {columnWidth, ParagraphAlignment.Center}
            });
            columns.Add("Unit Price" + (taxesEnabled ? " (ex TAX)" : ""), new Dictionary<string, ParagraphAlignment>
            {
                {columnWidth, ParagraphAlignment.Right}
            });
            if (taxesEnabled)
            {
                columns.Add("Tax Rate", new Dictionary<string, ParagraphAlignment>
                {
                    {columnWidth, ParagraphAlignment.Right}
                });
            }
            columns.Add("Net Sub Total", new Dictionary<string, ParagraphAlignment>
            {
                {"3cm", ParagraphAlignment.Right}
            });

            foreach (var item in columns)
            {
                Column column = table.AddColumn(item.Value.First().Key);
                column.Format.Alignment = item.Value.First().Value;
            }

            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = tableColor;
            row.TopPadding = 2;
            row.BottomPadding = 2;
            int rowId = 0;
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
            for (int i = 0; i < order.OrderLines.Count; i++)
            {
                OrderLine orderLine = order.OrderLines[i];
                Row row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;
                var counter = 0;
                row.Cells[counter++].AddParagraph((i + 1).ToString());
                row.Cells[counter++].AddParagraph(orderLine.Name);

                var quantity = orderLine.Quantity;
                row.Cells[counter++].AddParagraph(quantity.ToString());

                var taxesEnabled = _taxSettings.TaxesEnabled;
                var unitPrice = (taxesEnabled ? orderLine.UnitPricePreTax : orderLine.UnitPrice);
                row.Cells[counter++].AddParagraph(unitPrice.ToCurrencyFormat());

                if (taxesEnabled)
                {
                    row.Cells[counter++].AddParagraph((orderLine.TaxRate) + "%");
                }
                row.Cells[counter++].AddParagraph((unitPrice * quantity).ToCurrencyFormat());

                table.SetEdge(0, table.Rows.Count - 2, 5, 2, Edge.Box, BorderStyle.Single, 0.75);
            }
        }

        private void SetTableSummary(Order order, ref Table table)
        {
            var summaryData = new Dictionary<string, string>();
            var subtotal = _taxSettings.TaxesEnabled ? order.Subtotal : order.Total - order.ShippingTotal;
            summaryData.Add("Sub-total", subtotal.ToCurrencyFormat());
            var shipping = _taxSettings.TaxesEnabled && _taxSettings.ShippingRateTaxesEnabled
                ? (order.ShippingSubtotal - order.ShippingTax)
                : order.ShippingSubtotal;
            summaryData.Add("Shipping", shipping.ToCurrencyFormat());
            if (_taxSettings.TaxesEnabled)
            {
                summaryData.Add("Tax", order.Tax.ToCurrencyFormat());
            }
            summaryData.Add("Discount", order.DiscountAmount.ToCurrencyFormat());
            summaryData.Add("Total", order.Total.ToCurrencyFormat());

            var startIndex = _taxSettings.TaxesEnabled ? 5 : 4;
            foreach (var item in summaryData)
            {
                Row row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;
                row.Cells[0].Borders.Visible = false;
                row.Cells[0].AddParagraph(item.Key + ":");
                row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
                row.Cells[0].MergeRight = startIndex - 1;
                if (item.Key == "Total")
                    row.Cells[startIndex].Format.Font.Bold = true;
                row.Cells[startIndex].AddParagraph(item.Value);
            }

            table.SetEdge(startIndex, table.Rows.Count - 3, 1, 3, Edge.Box, BorderStyle.Single, 0.75);
        }

        private byte[] GetDocumentToByteArray(ref Document pdf)
        {
            var renderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Automatic) { Document = pdf };
            renderer.RenderDocument();
            var stream = new MemoryStream();

            if (String.IsNullOrWhiteSpace(_getLogoUrl.Get()) &&
                !String.IsNullOrWhiteSpace(_ecommerceSettings.ReportLogoImage))
            {
                XGraphics gfx = XGraphics.FromPdfPage(renderer.PdfDocument.Pages[0]);
                XImage image = FromUri(_ecommerceSettings.ReportLogoImage);
                if (image != null)
                {
                    gfx.DrawImage(image, 70, 50, image.PixelWidth, image.PixelHeight);
                    gfx.Save();
                }
            }
            renderer.PdfDocument.Save(stream);

            return stream.ToArray();
        }

        public XImage FromUri(string uri)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.AllowWriteStreamBuffering = true;
                WebResponse webResponse = webRequest.GetResponse();
                Image image = Image.FromStream(webResponse.GetResponseStream());
                Image thumbImg = ResizeImage(image, 150, 45);
                return XImage.FromGdiPlusImage(thumbImg);
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return null;
        }

        public Image ResizeImage(Image origImg, int width, int maxHeight)
        {
            int newHeight = origImg.Height * width / origImg.Width;
            if (newHeight > maxHeight)
            {
                width = origImg.Width * maxHeight / origImg.Height;
                newHeight = maxHeight;
            }
            Image newImg = origImg.GetThumbnailImage(width, newHeight, null, IntPtr.Zero);
            origImg.Dispose();
            return newImg;
        }
    }
}