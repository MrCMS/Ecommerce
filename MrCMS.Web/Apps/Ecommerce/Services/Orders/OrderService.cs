using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers;
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
        private readonly IOrderEventService _orderEventService;

        public OrderService(ISession session, IOrderEventService orderEventService)
        {
            _session = session;
            _orderEventService = orderEventService;
        }

        public Order PlaceOrder(CartModel cartModel, Action<Order> postCreationActions)
        {
            var placedOrder = _session.Transact(session =>
                                                    {
                                                        var order = new Order
                                                                        {
                                                                            ShippingAddress = cartModel.ShippingAddress.Clone(_session),
                                                                            BillingAddress = cartModel.BillingAddress.Clone(_session),
                                                                            ShippingMethod = cartModel.ShippingMethod,
                                                                            Subtotal = cartModel.Subtotal,
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
                                                                            PaymentMethod = cartModel.PaymentMethod,
                                                                            ShippingStatus = ShippingStatus.Pending,
                                                                            ShippingTaxPercentage = cartModel.ShippingTaxPercentage,
                                                                        };

                                                        foreach (var item in cartModel.Items)
                                                        {
                                                            var options = string.Join(", ", item.Item.AttributeValues.Select(value => value.FormattedValue));

                                                            order.OrderLines.Add(new OrderLine
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
                                                                                         Name = !string.IsNullOrEmpty(item.Item.Name) ? item.Item.Name : item.Item.Product.Name,
                                                                                         Options = options,
                                                                                         Discount =
                                                                                             item.GetDiscountAmount(cartModel.Discount, cartModel.DiscountCode),
                                                                                     });
                                                        }
                                                        postCreationActions(order);
                                                        session.SaveOrUpdate(order);
                                                        return order;
                                                    });
            _orderEventService.OrderPlaced(placedOrder);
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
            return _session.QueryOver<Order>().Where(x => x.User.Id == id &&
                x.OrderEmail.IsInsensitiveLike(email, MatchMode.Exact)).OrderBy(x => x.CreatedOn).Desc.Paged(pageNum, pageSize);
        }

        public void Cancel(Order order)
        {
            order.IsCancelled = true;
            _session.Transact(session => session.Update(order));
            _orderEventService.OrderCancelled(order);
        }

        public void MarkAsShipped(Order order)
        {
            order.ShippingDate = CurrentRequestData.Now;
            order.ShippingStatus = ShippingStatus.Shipped;
            _session.Transact(session => session.Update(order));
            _orderEventService.OrderShipped(order);
        }

        public void MarkAsPaid(Order order)
        {
            order.PaidDate = CurrentRequestData.Now;
            order.PaymentStatus = PaymentStatus.Paid;
            _session.Transact(session => session.Update(order));
        }

        public void MarkAsVoided(Order order)
        {
            order.PaymentStatus = PaymentStatus.Voided;
            _session.Transact(session => session.Update(order));
        }
    }
}