using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using NHibernate;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly GetCartImpl _getCartImpl;

        public OrderService(ISession session, GetCartImpl getCartImpl)
        {
            _session = session;
            _getCartImpl = getCartImpl;
        }

        public void PlaceOrder(CartModel cartModel)
        {
            var order = new Order
                            {
                                ShippingAddress = cartModel.ShippingAddress,
                                ShippingMethod = cartModel.ShippingMethod,
                                Subtotal = cartModel.Subtotal,
                                Discount = cartModel.Discount,
                                DiscountCode = cartModel.DiscountCode,
                                Tax = cartModel.Tax,
                                Total = cartModel.Total,
                                ShippingTotal = cartModel.ShippingTotal,
                                User = cartModel.User,
                                Weight = cartModel.Weight,
                                OrderEmail=cartModel.User!=null?cartModel.User.Email:String.Empty
                            };
            foreach (var item in cartModel.Items)
            {
                order.OrderLines.Add(new OrderLine()
                {
                UnitPrice=item.Price,
                Weight=item.Weight,
                TaxRate=item.TaxRatePercentage,
                Tax=item.Tax,
                Quantity=item.Quantity,
                ProductVariant=item.Item,
                Subtotal=item.PricePreTax*item.Quantity
                });
            }
            _session.Transact(session => session.Save(order));
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
    }
}