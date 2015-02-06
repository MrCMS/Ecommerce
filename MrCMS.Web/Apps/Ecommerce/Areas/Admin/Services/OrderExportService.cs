using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly ISession _session;
        private readonly IStringResourceProvider _stringResourceProvider;

        public OrderExportService(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _session = session;
            _stringResourceProvider = stringResourceProvider;
        }

        public FileResult ExportOrdersToExcel(OrderExportQuery exportQuery)
        {
            using (var excelFile = new ExcelPackage())
            {
                AddOrders(excelFile, exportQuery);
                AddOrderLines(excelFile, exportQuery);

                byte[] data = excelFile.GetAsByteArray();

                return new FileContentResult(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "mrcms-ecommerce-order-export.xlsx"
                };
            }
        }

        public OrderExportQuery GetDefaultQuery()
        {
            DateTime now = CurrentRequestData.Now;
            return new OrderExportQuery
            {
                From = now.AddMonths(-1),
                To = now
            };
        }

        private IList<Order> GetOrders(OrderExportQuery exportQuery)
        {
            var queryOver = GetOrderQuery(exportQuery);
            return queryOver.GetExecutableQueryOver(_session).Cacheable().List();
        }

        private QueryOver<Order, Order> GetOrderQuery(OrderExportQuery exportQuery)
        {
            return QueryOver.Of<Order>()
                .Where(order => order.OrderDate >= exportQuery.From &&
                                order.OrderDate <= exportQuery.To);
        }

        private IList<OrderLine> GetOrderLines(OrderExportQuery exportQuery)
        {
            return _session.QueryOver<OrderLine>()
                .WithSubquery.WhereProperty(line => line.Order.Id).In(
                    GetOrderQuery(exportQuery).Select(order => order.Id)
                ).Cacheable().List();
        }

        private void AddOrderLines(ExcelPackage excelFile, OrderExportQuery exportQuery)
        {
            ExcelWorksheet ordersWorksheet = excelFile.Workbook.Worksheets.Add("Order Lines");
            IList<OrderLine> orderLines = GetOrderLines(exportQuery);
            Dictionary<string, Func<OrderLine, object>> columns = GetOrderLineColumns();

            List<string> keys = columns.Keys.ToList();
            for (int index = 0; index < keys.Count; index++)
            {
                string key = keys[index];
                var cell = ordersWorksheet.Cells[1, index + 1];
                cell.Value = _stringResourceProvider.GetValue("Excel Order Line Export - " + key, key);
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            for (int i = 0; i < orderLines.Count; i++)
            {
                var orderLine = orderLines[i];
                for (int index = 0; index < keys.Count; index++)
                {
                    string key = keys[index];
                    var row = i + 2; // +1 for the non-zero-based index and +1 for the header row
                    var cell = ordersWorksheet.Cells[row, index + 1];
                    var value = columns[key](orderLine);
                    cell.Value = value;
                    if (value is DateTime)
                    {
                        cell.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                    }
                }
            }
            ordersWorksheet.Cells.AutoFitColumns();
        }

        private Dictionary<string, Func<OrderLine, object>> GetOrderLineColumns()
        {
            return new Dictionary<string, Func<OrderLine, object>>
            {
                {"Id", line => line.Id},
                {"Order Id", line => line.Order.Id},
                {"Name", line => line.Name},
                {"Options", line => line.Options},
                {"SKU", line => line.SKU},
                {"Quantity", line => line.Quantity},
                {"Unit Price", line => line.UnitPrice},
                {"Unit Price Pre Tax", line => line.UnitPricePreTax},
                {"Price", line => line.Price},
                {"Price Pre Tax", line => line.PricePreTax},
                {"Tax", line => line.Tax},
                {"Tax Rate", line => line.TaxRate},
                {"Discount", line => line.Discount},
                {"Weight", line => line.Weight},
                {"Subtotal", line => line.Subtotal},
                {"Requires Shipping", line => line.RequiresShipping},
                {"Is Downloadable", line => line.IsDownloadable},
                {"Allowed Number Of Downloads", line => line.AllowedNumberOfDownloads},
                {"Download Expires On", line => line.DownloadExpiresOn},
                {"Number Of Downloads", line => line.NumberOfDownloads},
                {"Download File URL", line => line.DownloadFileUrl},
                {"Download File Content Type", line => line.DownloadFileContentType},
                {"Download File Name", line => line.DownloadFileName},
                {"Data", line => line.Data},
            };
        }

        private void AddOrders(ExcelPackage excelFile, OrderExportQuery exportQuery)
        {
            ExcelWorksheet ordersWorksheet = excelFile.Workbook.Worksheets.Add("Orders");
            IList<Order> orders = GetOrders(exportQuery);
            Dictionary<string, Func<Order, object>> columns = GetOrderColumns();

            List<string> keys = columns.Keys.ToList();
            for (int index = 0; index < keys.Count; index++)
            {
                string key = keys[index];
                var cell = ordersWorksheet.Cells[1, index + 1];
                cell.Value = _stringResourceProvider.GetValue("Excel Order Export - " + key, key);
                cell.Style.Font.Bold = true;
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                for (int index = 0; index < keys.Count; index++)
                {
                    string key = keys[index];
                    var row = i + 2; // +1 for the non-zero-based index and +1 for the header row
                    var cell = ordersWorksheet.Cells[row, index + 1];
                    var value = columns[key](order);
                    cell.Value = value;
                    if (value is DateTime)
                    {
                        cell.Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                    }
                }
            }
            ordersWorksheet.Cells.AutoFitColumns();
        }

        public Dictionary<string, Func<Order, object>> GetOrderColumns()
        {
            var columns = new Dictionary<string, Func<Order, object>>
            {
                {"Id", order => order.Id},
                {"Guid", order => order.Guid},
                {"Date", order => order.OrderDate.GetValueOrDefault(order.CreatedOn)},
                {"Email", order => order.OrderEmail},
                {"Subtotal", order => order.Subtotal},
                {"Tax", order => order.Tax},
                {"Total", order => order.Total},
                {"Total Paid", order => order.TotalPaid},
                {"Payment Method", order => order.PaymentMethod},
                {"Payment Status", order => order.PaymentStatus.ToString().BreakUpString()},
                {"Paid Date", order => order.PaidDate},
                //{"Total Refunds", order => order.TotalRefunds},
                //{"Total After Refunds", order => order.TotalAfterRefunds},
                {"Discount Amount", order => order.DiscountAmount},
                {"Reward Points Applied", order => order.RewardPointsAppliedAmount},
                {"Weight", order => order.Weight},
                {"Shipping Method", order => order.ShippingMethodName},
                {"Shipping Subtotal", order => order.ShippingSubtotal},
                {"Shipping Tax", order => order.ShippingTax},
                {"Shipping Discount Amount", order => order.ShippingDiscountAmount},
                {"Shipping Total", order => order.ShippingTotal},
                {"Shipping Tax Percentage", order => order.ShippingTaxPercentage},
                {"Shipping Status", order => order.ShippingStatus.ToString().BreakUpString()},
                {
                    "Requested Shipping Date",
                    order =>
                        order.RequestedShippingDate.HasValue
                            ? (object) order.RequestedShippingDate.Value
                            : "ASAP"
                },
                {"Shipping Date", order => order.ShippingDate},
                {"Shipping Company", order => order.ShippingAddress != null ? order.ShippingAddress.Company : null},
                {"Shipping Title", order => order.ShippingAddress != null ? order.ShippingAddress.Title : null},
                {"Shipping First Name", order => order.ShippingAddress != null ? order.ShippingAddress.FirstName : null},
                {"Shipping Last Name", order => order.ShippingAddress != null ? order.ShippingAddress.LastName : null},
                {"Shipping Address 1", order => order.ShippingAddress != null ? order.ShippingAddress.Address1 : null},
                {"Shipping Address 2", order => order.ShippingAddress != null ? order.ShippingAddress.Address2 : null},
                {"Shipping City", order => order.ShippingAddress != null ? order.ShippingAddress.City : null},
                {"Shipping State/Province", order => order.ShippingAddress != null ? order.ShippingAddress.StateProvince : null},
                {"Shipping Country", order => order.ShippingAddress != null ? order.ShippingAddress.GetCountryName() : null},
                {"Shipping Postal Code", order => order.ShippingAddress != null ? order.ShippingAddress.PostalCode : null},
                {"Shipping Phone Number", order => order.ShippingAddress != null ? order.ShippingAddress.PhoneNumber : null},
                {"Billing Company", order => order.BillingAddress != null ? order.BillingAddress.Company : null},
                {"Billing Title", order => order.BillingAddress != null ? order.BillingAddress.Title : null},
                {"Billing First Name", order => order.BillingAddress != null ? order.BillingAddress.FirstName : null},
                {"Billing Last Name", order => order.BillingAddress != null ? order.BillingAddress.LastName : null},
                {"Billing Address 1", order => order.BillingAddress != null ? order.BillingAddress.Address1 : null},
                {"Billing Address 2", order => order.BillingAddress != null ? order.BillingAddress.Address2 : null},
                {"Billing City", order => order.BillingAddress != null ? order.BillingAddress.City : null},
                {"Billing State/Province", order => order.BillingAddress != null ? order.BillingAddress.StateProvince : null},
                {"Billing Country", order => order.BillingAddress != null ? order.BillingAddress.GetCountryName() : null},
                {"Billing Postal Code", order => order.BillingAddress != null ? order.BillingAddress.PostalCode : null},
                {"Billing Phone Number", order => order.BillingAddress != null ? order.BillingAddress.PhoneNumber : null},
                {"Customer IP", order => order.CustomerIP},
                {"Authorisation Token", order => order.AuthorisationToken},
                {"Capture Transaction Id", order => order.CaptureTransactionId},
                {"Is Cancelled?", order => order.IsCancelled},
                {"Tracking Number", order => order.TrackingNumber},
                {"Sales Channel", order => order.SalesChannel},
                {"Gift Message", order => order.GiftMessage},
                {"Order Status", order => order.OrderStatus},
            };
            return columns;
        }
    }
}