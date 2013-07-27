using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Entities.People;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly ICartManager _cartManager;

        public OrderService(ISession session, ICartManager cartManager)
        {
            _session = session;
            _cartManager = cartManager;
        }

        public Order PlaceOrder(CartModel cartModel, Action<Order> postCreationActions)
        {
            return _session.Transact(session =>
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
                                           var options = string.Join(", ",
                                                                     item.Item.AttributeValues.Select(
                                                                         value => value.FormattedValue));

                                           order.OrderLines.Add(new OrderLine
                                                                    {
                                                                        Order = order,
                                                                        UnitPrice = item.Price,
                                                                        Weight = item.Weight,
                                                                        TaxRate = item.TaxRatePercentage,
                                                                        Tax = item.Tax,
                                                                        Quantity = item.Quantity,
                                                                        ProductVariant = item.Item,
                                                                        Subtotal = item.PricePreTax * item.Quantity,
                                                                        SKU = item.Item.SKU,
                                                                        Name =
                                                                            !string.IsNullOrEmpty(item.Item.Name)
                                                                                ? item.Item.Name
                                                                                : item.Item.Product.Name,
                                                                        Options = options,
                                                                        Discount =
                                                                            item.GetDiscountAmount(cartModel.Discount,
                                                                                                   cartModel.DiscountCode),
                                                                    });
                                       }
                                       postCreationActions(order);
                                       session.SaveOrUpdate(order);
                                       _cartManager.EmptyBasket();
                                       return order;
                                   });
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
    }
}