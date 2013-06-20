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
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly ISession _session;
        private readonly GetCartImpl _getCartImpl;
        private readonly ICountryService _countryService;
        private readonly ICartManager _cartManager;

        public OrderService(ISession session, GetCartImpl getCartImpl,ICountryService countryService,ICartManager cartManager)
        {
            _session = session;
            _getCartImpl = getCartImpl;
            _countryService = countryService;
            _cartManager = cartManager;
        }

        public int PlaceOrder(CartModel cartModel)
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
            string IP = String.Empty;
            string rawIP = CurrentRequestData.CurrentContext.Request.ServerVariables["HTTP_X_FORWARD_FOR"];
            if (!String.IsNullOrEmpty(rawIP))
            {
                String[] ipAddress = rawIP.Split(',');
                if (ipAddress.Length != 0)
                {
                    IP = ipAddress[0];
                }
            }
            else
                IP = CurrentRequestData.CurrentContext.Request.ServerVariables["REMOTE_ADDR"];

            var order = new Order
                            {
                                ShippingAddress=cartModel.ShippingAddress,
                                BillingAddress=cartModel.BillingAddress,
                                ShippingMethod = cartModel.ShippingMethod,
                                Subtotal = cartModel.Subtotal,
                                Discount = cartModel.Discount,
                                DiscountCode = cartModel.DiscountCode,
                                Tax = cartModel.Tax,
                                Total = cartModel.Total,
                                ShippingTotal = cartModel.ShippingTotal,
                                User = cartModel.User,
                                Weight = cartModel.Weight,
                                OrderEmail=cartModel.OrderEmail,
                                CustomerIP=IP
                            };
            _session.Transact(session => session.SaveOrUpdate(order));
            order = Get(order.Id); 
            foreach (var item in cartModel.Items)
            {
                order.OrderLines.Add(new OrderLine()
                {
                    Order=order,
                    UnitPrice=item.Price,
                    Weight=item.Weight,
                    TaxRate=item.TaxRatePercentage,
                    Tax=item.Tax,
                    Quantity=item.Quantity,
                    ProductVariant=item.Item,
                    Subtotal=item.PricePreTax*item.Quantity
                });
            }
            _session.Transact(session => session.SaveOrUpdate(order));
            _cartManager.EmptyBasket();
            return order.Id;
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