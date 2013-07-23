using System;
using System.Linq;
using MrCMS.Helpers;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Entities.People;
using NHibernate.Criterion;
using Order = MrCMS.Web.Apps.Ecommerce.Entities.Orders.Order;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly ICountryService _countryService;
        private readonly ICartManager _cartManager;

        public OrderService(ISession session, ICountryService countryService, ICartManager cartManager)
        {
            _session = session;
            _countryService = countryService;
            _cartManager = cartManager;
        }

        public Order PlaceOrder(CartModel cartModel)
        {
            cartModel.ShippingAddress.Site = CurrentRequestData.CurrentSite;
            cartModel.BillingAddress.Site = CurrentRequestData.CurrentSite;

            //WE NEED TO DO THIS, BECAUSE OF THE 
            //a different object with the same identifier value was already associated with the session: 1
            //ERROR
            cartModel.ShippingAddress.Country = _countryService.Get(cartModel.ShippingAddress.Country.Id);
            cartModel.BillingAddress.Country = _countryService.Get(cartModel.BillingAddress.Country.Id);

            if (cartModel.ShippingAddress != null && cartModel.ShippingAddress == cartModel.BillingAddress)
                _session.Transact(session => session.SaveOrUpdate(cartModel.ShippingAddress));
            else
            {
                _session.Transact(session => session.SaveOrUpdate(cartModel.ShippingAddress));
                _session.Transact(session => session.SaveOrUpdate(cartModel.BillingAddress));
            }

            //IP
            var ip = String.Empty;
            var rawIp = CurrentRequestData.CurrentContext.Request.ServerVariables["HTTP_X_FORWARD_FOR"];
            if (!String.IsNullOrEmpty(rawIp))
            {
                String[] ipAddress = rawIp.Split(',');
                if (ipAddress.Length != 0)
                {
                    ip = ipAddress[0];
                }
            }
            else
                ip = CurrentRequestData.CurrentContext.Request.ServerVariables["REMOTE_ADDR"];

            var order = new Order
                            {
                                ShippingAddress = cartModel.ShippingAddress,
                                BillingAddress = cartModel.BillingAddress,
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
                                CustomerIP = ip
                            };

            foreach (var item in cartModel.Items)
            {
                var options = string.Join(", ", item.Item.AttributeValues.Select(value => value.FormattedValue));

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
                        Name = !string.IsNullOrEmpty(item.Item.Name) ? item.Item.Name : item.Item.Product.Name,
                        Options = options
                    });
            }
            _session.Transact(session => session.SaveOrUpdate(order));
            _cartManager.EmptyBasket();
            return order;
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