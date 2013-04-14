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
            Order order = new Order();
            order.ShippingAddress = cartModel.ShippingAddress;
            order.ShippingMethod = cartModel.ShippingMethod;
            order.Subtotal = cartModel.Subtotal;
            order.Discount = cartModel.Discount;
            order.DiscountCode = cartModel.DiscountCode;
            order.Tax = cartModel.Tax;
            order.Total = cartModel.Total;
            order.ShippingTotal = cartModel.ShippingTotal;
            order.User = cartModel.User;
            order.Weight = cartModel.Weight;
            _session.Transact(session => session.Save(order));
        }

        public void Save(Order item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public IPagedList<Order> GetAll(int pageNum, int pageSize = 10)
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
    }
}