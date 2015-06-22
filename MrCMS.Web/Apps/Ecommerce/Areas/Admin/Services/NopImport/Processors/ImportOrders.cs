using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Events;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;
using PaymentStatus = MrCMS.Web.Apps.Ecommerce.Models.PaymentStatus;
using ShippingStatus = MrCMS.Web.Apps.Ecommerce.Models.ShippingStatus;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services.NopImport.Processors
{
    public class ImportOrders : IImportOrders
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public ImportOrders(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string ProcessOrders(NopCommerceDataReader dataReader, NopImportContext nopImportContext)
        {
            HashSet<OrderData> orders = dataReader.GetOrderData();
            var orderLines = dataReader.GetOrderLineData().GroupBy(x => x.OrderId)
                .ToDictionary(x => x.Key, x => x.ToHashSet());
            var site = _session.Get<Site>(_site.Id);
            var guids = _session.QueryOver<Order>().Select(o => o.Guid).List<Guid>();

            using (EventContext.Instance.Disable<GenerateGiftCards>())
            {
                _session.Transact(session =>
                {
                    foreach (OrderData data in orders)
                    {
                        Guid guid = data.Guid;
                        if (guids.Contains(guid))
                            continue;

                        Entities.Orders.AddressData billingAddressData = null;
                        if (data.BillingAddress != null)
                        {
                            billingAddressData = new Entities.Orders.AddressData
                            {
                                Address1 = data.BillingAddress.Address1,
                                Address2 = data.BillingAddress.Address2,
                                City = data.BillingAddress.City,
                                Company = data.BillingAddress.Company,
                                CountryCode = data.BillingAddress.CountryCode,
                                FirstName = data.BillingAddress.FirstName,
                                LastName = data.BillingAddress.LastName,
                                PhoneNumber = data.BillingAddress.PhoneNumber,
                                PostalCode = data.BillingAddress.PostalCode,
                                StateProvince = data.BillingAddress.StateProvince
                            };
                        }


                        Entities.Orders.AddressData shippingAddressData = null;
                        if (data.ShippingAddress != null)
                        {
                            shippingAddressData = new Entities.Orders.AddressData
                            {
                                Address1 = data.ShippingAddress.Address1,
                                Address2 = data.ShippingAddress.Address2,
                                City = data.ShippingAddress.City,
                                Company = data.ShippingAddress.Company,
                                CountryCode = data.ShippingAddress.CountryCode,
                                FirstName = data.ShippingAddress.FirstName,
                                LastName = data.ShippingAddress.LastName,
                                PhoneNumber = data.ShippingAddress.PhoneNumber,
                                PostalCode = data.ShippingAddress.PostalCode,
                                StateProvince = data.ShippingAddress.StateProvince
                            };
                        }

                        var order = new Order
                        {
                            BillingAddress = billingAddressData,
                            ShippingAddress = shippingAddressData,
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
                        order.AssignBaseProperties(site);
                        session.Insert(order);

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
                            orderNote.AssignBaseProperties(site);
                            session.Insert(orderNote);
                        }
                        int orderId = data.Id;
                        HashSet<OrderLineData> lineDatas = orderLines.ContainsKey(orderId)
                            ? orderLines[orderId]
                            : new HashSet<OrderLineData>();
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
                            orderLine.AssignBaseProperties(site);
                            session.Insert(orderLine);
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