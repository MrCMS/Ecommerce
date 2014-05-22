using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Entities.People;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly IOrderNoteService _orderNoteService;
        private readonly IFileService _fileService;

        public OrderService(ISession session, IOrderNoteService orderNoteService, IFileService fileService)
        {
            _session = session;
            _orderNoteService = orderNoteService;
            _fileService = fileService;
        }

        public Order PlaceOrder(CartModel cartModel, Action<Order> postCreationActions)
        {
            // this code is here to try and take into account a 3rd party posting back more than once for whatever reason (see bug #2754). 
            // If the order with the guid has already been placed, we'll just return that placed order
            var existingOrders = _session.QueryOver<Order>().Where(order => order.Guid == cartModel.CartGuid).List();
            if (existingOrders.Any())
            {
                return existingOrders.First();
            }
            var placedOrder = _session.Transact(session =>
                                                    {
                                                        var order = new Order
                                                                        {
                                                                            ShippingAddress = cartModel.RequiresShipping ? cartModel.ShippingAddress.ToAddressData(_session) : null,
                                                                            BillingAddress = cartModel.BillingAddress.ToAddressData(_session),
                                                                            ShippingMethod = cartModel.ShippingMethod,
                                                                            ShippingMethodName = cartModel.RequiresShipping ? cartModel.ShippingMethod.Name : "No shipping required",
                                                                            Subtotal = cartModel.Subtotal,
                                                                            DiscountAmount = cartModel.OrderTotalDiscount,
                                                                            Discount = cartModel.Discount,
                                                                            DiscountCode = cartModel.DiscountCode,
                                                                            Tax = cartModel.Tax,
                                                                            Total = cartModel.Total,
                                                                            ShippingTotal = cartModel.ShippingTotal,
                                                                            ShippingTax = cartModel.ShippingTax,
                                                                            User = cartModel.User,
                                                                            Weight = cartModel.Weight,
                                                                            OrderEmail = cartModel.OrderEmail,
                                                                            CustomerIP = RequestHelper.GetIP(),
                                                                            HttpData = RequestHelper.GetRawHttpData(),
                                                                            PaymentMethod = cartModel.PaymentMethodSystemName,
                                                                            ShippingStatus = cartModel.RequiresShipping ? ShippingStatus.Pending : ShippingStatus.ShippingNotRequired,
                                                                            ShippingTaxPercentage = cartModel.ShippingTaxPercentage,
                                                                            SalesChannel = EcommerceApp.DefaultSalesChannel,
                                                                            Guid = cartModel.CartGuid
                                                                        };

                                                        foreach (var item in cartModel.Items)
                                                        {
                                                            var options = string.Join(", ", item.Item.OptionValues.Select(value => value.FormattedValue));

                                                            var orderLine = new OrderLine
                                                                                {
                                                                                    Order = order,
                                                                                    UnitPrice = item.UnitPrice,
                                                                                    UnitPricePreTax = item.UnitPricePreTax,
                                                                                    Weight = item.Weight,
                                                                                    TaxRate = item.TaxRatePercentage,
                                                                                    Tax = item.Tax,
                                                                                    Quantity = item.Quantity,
                                                                                    ProductVariant = item.Item,
                                                                                    PricePreTax = item.PricePreTax,
                                                                                    Price = item.Price,
                                                                                    SKU = item.Item.SKU,
                                                                                    Name = item.Item.FullName,
                                                                                    Options = options,
                                                                                    Discount = item.DiscountAmount,
                                                                                    RequiresShipping = item.RequiresShipping
                                                                                };
                                                            if (item.IsDownloadable)
                                                            {
                                                                orderLine.IsDownloadable = true;
                                                                orderLine.AllowedNumberOfDownloads = item.AllowedNumberOfDownloads;
                                                                orderLine.DownloadExpiresOn =
                                                                    (item.AllowedNumberOfDaysForDownload.HasValue && item.AllowedNumberOfDaysForDownload > 0)
                                                                        ? CurrentRequestData.Now.AddDays(
                                                                            item.AllowedNumberOfDaysForDownload
                                                                                .GetValueOrDefault())
                                                                        : (DateTime?)null;
                                                                orderLine.NumberOfDownloads = 0;
                                                                var fileByUrl = _fileService.GetFileByUrl(item.DownloadFileUrl);
                                                                if (fileByUrl != null)
                                                                {
                                                                    orderLine.DownloadFileUrl = fileByUrl.FileUrl;
                                                                    orderLine.DownloadFileContentType = fileByUrl.ContentType;
                                                                    orderLine.DownloadFileName = fileByUrl.FileName;
                                                                }
                                                                else
                                                                {
                                                                    orderLine.DownloadFileUrl = item.DownloadFileUrl;
                                                                }
                                                            }
                                                            order.OrderLines.Add(orderLine);
                                                        }
                                                        if (postCreationActions != null)
                                                            postCreationActions(order);

                                                        // Similarly, we check again just before we save - we should be fine as we are inside of a transaction
                                                        // but we will err on the side of catching duplicates
                                                        existingOrders = _session.QueryOver<Order>().Where(o => o.Guid == cartModel.CartGuid).List();
                                                        if (existingOrders.Any())
                                                        {
                                                            return existingOrders.First();
                                                        }

                                                        session.SaveOrUpdate(order);

                                                        return order;
                                                    });

            EventContext.Instance.Publish<IOnOrderPlaced, OrderPlacedArgs>(new OrderPlacedArgs { Order = placedOrder });
            return placedOrder;
        }

        public IPagedList<Order> GetPaged(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Paged(pageNum, pageSize);
        }

        private IQueryOver<Order, Order> BaseQuery()
        {
            return
                _session.QueryOver<Order>()
                        .OrderBy(entry => entry.CreatedOn)
                        .Desc;
        }

        public void Save(Order item)
        {
            item.IsCancelled = false;
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public Order Get(int id)
        {
            return _session.QueryOver<Order>().Where(x => x.Id == id).Cacheable().SingleOrDefault();
        }

        public IPagedList<Order> GetOrdersByUser(User user, int pageNum, int pageSize = 10)
        {
            var id = user.Id;
            var email = user.Email;
            return _session.QueryOver<Order>().Where(x => x.User.Id == id).OrderBy(x => x.CreatedOn).Desc.Paged(pageNum, pageSize);
        }

        public IList<Order> GetOrdersByUser(User user)
        {
            var id = user.Id;
            var email = user.Email;
            return _session.QueryOver<Order>().Where(x => x.User.Id == id ||
                x.OrderEmail.IsInsensitiveLike(email, MatchMode.Exact)).OrderBy(x => x.CreatedOn).Desc.Cacheable().List();
        }

        public void Cancel(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as cancelled by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.IsCancelled = true;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderCancelled, OrderCancelledArgs>(new OrderCancelledArgs { Order = order });
        }

        public void MarkAsShipped(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as shipped by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.ShippingDate = CurrentRequestData.Now;
            order.ShippingStatus = ShippingStatus.Shipped;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderShipped, OrderShippedArgs>(new OrderShippedArgs { Order = order });
        }

        public void MarkAsPaid(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as paid by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.PaidDate = CurrentRequestData.Now;
            order.PaymentStatus = PaymentStatus.Paid;
            _session.Transact(session => session.Update(order));
            EventContext.Instance.Publish<IOnOrderPaid, OrderPaidArgs>(new OrderPaidArgs { Order = order });
        }

        public void MarkAsVoided(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order payment marked as void by {0}.",
                CurrentRequestData.CurrentUser != null ? CurrentRequestData.CurrentUser.Name : "System"), order);

            order.PaymentStatus = PaymentStatus.Voided;
            _session.Transact(session => session.Update(order));
        }

        public Order SetLastOrderUserIdByOrderId(int orderId)
        {
            var order = _session.Get<Order>(orderId);

            if (order == null) return null;

            order.User = CurrentRequestData.CurrentUser;
            _session.Transact(session => session.Update(order));
            return order;
        }

        public Order GetByGuid(Guid id)
        {
            return _session.QueryOver<Order>().Where(order => order.Guid == id).Take(1).Cacheable().SingleOrDefault();
        }

        public void Delete(Order order)
        {
            _orderNoteService.AddOrderNoteAudit(string.Format("Order marked as deleted by {0}.",
                                                                       CurrentRequestData.CurrentUser.Name), order);
            _session.Transact(session => session.Delete(order));
        }
    }
}