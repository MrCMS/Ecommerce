using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Events;
using NHibernate;
using PaymentStatus = MrCMS.Web.Apps.Ecommerce.Models.PaymentStatus;
using ShippingStatus = MrCMS.Web.Apps.Ecommerce.Models.ShippingStatus;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportOrders : IImportOrders
    {
        private readonly ISession _session;

        public ImportOrders(ISession session)
        {
            _session = session;
        }

        public string ProcessOrders(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<OrderData> orders = dataReader.GetOrderData();
            HashSet<OrderLineData> orderLines = dataReader.GetOrderLineData();

            using (EventContext.Instance.Disable<GenerateGiftCards>())
            {
                _session.Transact(session =>
                {
                    foreach (OrderData data in orders)
                    {
                        Guid guid = data.Guid;
                        if (session.QueryOver<Order>().Where(o => o.Guid == guid).Any())
                            continue;

                        var billingAddress = nopImportContext.FindNew<Address>(data.BillingAddressId);
                        var shippingAddress =
                            nopImportContext.FindNew<Address>(data.ShippingAddressId.GetValueOrDefault());
                        var order = new Order
                        {
                            BillingAddress = billingAddress.ToAddressData(),
                            ShippingAddress = shippingAddress != null ? shippingAddress.ToAddressData() : null,
                            CustomerIP = data.CustomerIp,
                            DiscountAmount = data.OrderDiscount,
                            Id = data.Id,
                            OrderEmail = data.Email,
                            PaidDate = data.PaidDate,
                            PaymentStatus = GetPaymentStatus(data.PaymentStatus),
                            ShippingStatus = GetShippingStatus(data.ShippingStatus),
                            ShippingMethodName = data.ShippingMethodName,
                            ShippingSubtotal = data.OrderShippingExclTax,
                            ShippingTax = data.OrderShippingInclTax - data.OrderShippingExclTax,
                            ShippingTotal = data.OrderShippingInclTax,
                            Subtotal = data.OrderSubtotalInclTax,
                            Tax = data.OrderTax,
                            Total = data.OrderTotal,
                            TotalPaid = data.OrderTotal,
                            User = nopImportContext.FindNew<User>(data.CustomerId),
                            SalesChannel = EcommerceApp.NopCommerceSalesChannel,
                            PaymentMethod = data.PaymentMethod,
                            OrderDate = data.OrderDate
                        };
                        order.SetGuid(data.Guid);
                        session.Save(order);

                        if (order.OrderNotes == null)
                            order.OrderNotes = new List<OrderNote>();
                        data.Notes.Add(new OrderNoteData
                        {
                            Note = "Previous order id: " + data.Id,
                            ShowToCustomer = false
                        });
                        foreach (OrderNoteData note in data.Notes)
                        {
                            var orderNote = new OrderNote
                            {
                                ShowToClient = note.ShowToCustomer,
                                Note = note.Note,
                                Order = order
                            };
                            if (note.Date.HasValue)
                                orderNote.CreatedOn = note.Date.Value;
                            order.OrderNotes.Add(orderNote);
                            session.Save(orderNote);
                        }
                        int orderId = data.Id;
                        HashSet<OrderLineData> lineDatas = orderLines.FindAll(x => x.OrderId == orderId);
                        foreach (OrderLineData lineData in lineDatas)
                        {
                            var orderLine = new OrderLine
                            {
                                Discount = lineData.DiscountAmountInclTax,
                                Id = lineData.Id,
                                Order = order,
                                Quantity = lineData.Quantity,
                                Name = lineData.ProductName,
                                Price = lineData.PriceInclTax,
                                UnitPrice = lineData.UnitPriceInclTax,
                                UnitPricePreTax = lineData.DiscountAmountExclTax,
                                PricePreTax = lineData.PriceExclTax,
                                Tax = lineData.PriceInclTax - lineData.PriceExclTax,
                                Weight = lineData.ItemWeight.GetValueOrDefault(),
                                SKU = lineData.SKU,
                                RequiresShipping = lineData.RequiresShipping
                            };
                            order.OrderLines.Add(orderLine);
                            session.Save(orderLine);
                        }
                        nopImportContext.AddEntry(data.Id, order);
                    }
                });
            }

            return string.Format("{0} orders processed", orders.Count);
        }


        private ShippingStatus GetShippingStatus(Models.ShippingStatus shippingStatus)
        {
            switch (shippingStatus)
            {
                case Models.ShippingStatus.ShippingNotRequired:
                    return ShippingStatus.ShippingNotRequired;
                case Models.ShippingStatus.NotYetShipped:
                    return ShippingStatus.Pending;
                case Models.ShippingStatus.PartiallyShipped:
                    return ShippingStatus.PartiallyShipped;
                case Models.ShippingStatus.Shipped:
                case Models.ShippingStatus.Delivered:
                    return ShippingStatus.Shipped;
                default:
                    throw new ArgumentOutOfRangeException("shippingStatus");
            }
        }

        private PaymentStatus GetPaymentStatus(Models.PaymentStatus paymentStatus)
        {
            switch (paymentStatus)
            {
                case Models.PaymentStatus.Pending:
                case Models.PaymentStatus.Authorized:
                    return PaymentStatus.Pending;
                case Models.PaymentStatus.Paid:
                    return PaymentStatus.Paid;
                case Models.PaymentStatus.PartiallyRefunded:
                    return PaymentStatus.PartiallyRefunded;
                case Models.PaymentStatus.Refunded:
                    return PaymentStatus.Refunded;
                case Models.PaymentStatus.Voided:
                    return PaymentStatus.Voided;
                default:
                    throw new ArgumentOutOfRangeException("paymentStatus");
            }
        }
    }
}