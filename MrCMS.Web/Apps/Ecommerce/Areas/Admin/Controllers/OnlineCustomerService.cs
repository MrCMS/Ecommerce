using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OnlineCustomerService : IOnlineCustomerService
    {
        private readonly ISession _session;

        public OnlineCustomerService(ISession session)
        {
            _session = session;
        }

        public IPagedList<OnlineCustomerCart> Search(OnlineCustomerSearchQuery query)
        {
            var sessionDataQuery = GetSessionDataQuery(query).Cacheable();
            var sessionData = sessionDataQuery.List<SessionDataQueryResult>();
            var userCartItemCounts = GetUserCartItemCounts(sessionData);

            var results = sessionData.Select(result =>
            {
                var itemQueryResult = userCartItemCounts.FirstOrDefault(x => x.UserGuid == result.UserGuid);
                return new OnlineCustomerCart
                {
                    UserId = result.UserId,
                    UserGuid = result.UserGuid,
                    FirstName = result.FirstName ?? string.Empty,
                    LastName = result.LastName ?? string.Empty,
                    Email = result.Email ?? string.Empty,
                    ItemsCount = itemQueryResult?.Count ?? 0,
                    LastUpdatedOn = result.UpdatedOn
                };
            });

            return results.Where(x => x.ItemsCount > 0).ToPagedList(query.Page, 10);
        }

        public OnlineCustomerCart GetCart(Guid userGuid)
        {
            var sessionData = GetSessionDataQuery().Where(x => x.UserGuid == userGuid).Cacheable()
                .List<SessionDataQueryResult>().FirstOrDefault();
            var cartItems = GetCartItems(userGuid);

            return new OnlineCustomerCart
            {
                UserGuid = userGuid,
                UserId = sessionData.UserId,
                FirstName = sessionData.FirstName,
                LastName = sessionData.LastName,
                Email = sessionData.Email,
                ShippingAddress = sessionData.ShippingAddress,
                BillingAddress = sessionData.BillingAddress,
                BillingAddressIsSameAsShipping = sessionData.BillingAddressIsSameAsShipping,
                Items = cartItems.ToList(),
                SubTotal = cartItems.Sum(x => x.Product.BasePrice),
                ItemsCount = cartItems.Count,
                LastUpdatedOn = sessionData.UpdatedOn
            };
        }

        private IList<OnlineCustomerCartItem> GetCartItems(Guid userGuid)
        {
            OnlineCustomerCartItem cartQueryResult = null;
            CartItem cartItemAlias = null;

            return _session.QueryOver(() => cartItemAlias)
                .Where(x => x.UserGuid == userGuid)
                .SelectList(builder =>
                {
                    builder.Select(() => cartItemAlias.Item).WithAlias(() => cartQueryResult.Product);
                    builder.Select(() => cartItemAlias.Quantity).WithAlias(() => cartQueryResult.Quantity);

                    return builder;
                }).TransformUsing(Transformers.AliasToBean<OnlineCustomerCartItem>())
                .Cacheable()
                .List<OnlineCustomerCartItem>();
        }

        private IList<UserCartItemCountQueryResult> GetUserCartItemCounts(IList<SessionDataQueryResult> sessionData)
        {
            UserCartItemCountQueryResult cartItemQueryResult = null;
            var userGuids = sessionData.Select(x => x.UserGuid).ToList();
            return _session.QueryOver<CartItem>().Where(x => x.UserGuid.IsIn(userGuids))
                .SelectList(builder =>
                {
                    builder.SelectGroup(x => x.UserGuid).WithAlias(() => cartItemQueryResult.UserGuid);
                    builder.SelectCount(x => x.Id).WithAlias(() => cartItemQueryResult.Count);

                    return builder;
                }).TransformUsing(Transformers.AliasToBean<UserCartItemCountQueryResult>())
                .Cacheable()
                .List<UserCartItemCountQueryResult>();
        }

        private IQueryOver<SessionData, SessionData> GetSessionDataQuery(OnlineCustomerSearchQuery query = null)
        {
            User userAlias = null;
            SessionData sessionDataAlias = null;
            SessionDataQueryResult sessionDataQueryResult = null;

            var queryOver = _session.QueryOver(() => sessionDataAlias)
                .JoinAlias(x => x.User, () => userAlias, JoinType.LeftOuterJoin)
                .Where(x => x.Key == CartManager.CurrentCartGuid)
                .SelectList(builder =>
                {
                    builder.Select(() => userAlias.FirstName).WithAlias(() => sessionDataQueryResult.FirstName);
                    builder.Select(() => userAlias.LastName).WithAlias(() => sessionDataQueryResult.LastName);
                    builder.Select(() => userAlias.Email).WithAlias(() => sessionDataQueryResult.UserEmail);
                    builder.Select(data => data.UserGuid).WithAlias(() => sessionDataQueryResult.UserGuid);
                    builder.Select(() => userAlias.Id).WithAlias(() => sessionDataQueryResult.UserId);
                    builder.Select(data => data.UpdatedOn).WithAlias(() => sessionDataQueryResult.UpdatedOn);

                    builder.SelectSubQuery(QueryOver.Of<SessionData>()
                        .Where(x => x.UserGuid == sessionDataAlias.UserGuid && x.Key == CartManager.CurrentOrderEmailKey)
                        .Select(x => x.Data)
                        .Take(1)).WithAlias(() => sessionDataQueryResult.SessionEmail);

                    builder.SelectSubQuery(QueryOver.Of<SessionData>()
                        .Where(x => x.UserGuid == sessionDataAlias.UserGuid && x.Key == CartManager.CurrentShippingAddressKey)
                        .Select(x => x.Data)
                        .Take(1)).WithAlias(() => sessionDataQueryResult.SessionShippingAddress);

                    builder.SelectSubQuery(QueryOver.Of<SessionData>()
                        .Where(x => x.UserGuid == sessionDataAlias.UserGuid && x.Key == CartManager.CurrentBillingAddressKey)
                        .Select(x => x.Data)
                        .Take(1)).WithAlias(() => sessionDataQueryResult.SessionBillingAddress);

                    builder.SelectSubQuery(QueryOver.Of<SessionData>()
                        .Where(x => x.UserGuid == sessionDataAlias.UserGuid && x.Key == CartManager.CurrentBillingAddressSameAsShippingAddressKey)
                        .Select(x => x.Data)
                        .Take(1)).WithAlias(() => sessionDataQueryResult.BillingAddressIsSameAsShipping);

                    return builder;
                });

            if (query != null && !string.IsNullOrWhiteSpace(query.FirstName))
            {
                queryOver = queryOver.Where(() => userAlias.FirstName.IsInsensitiveLike(query.FirstName));
            }

            if (query != null && !string.IsNullOrWhiteSpace(query.LastName))
            {
                queryOver = queryOver.Where(() => userAlias.LastName.IsInsensitiveLike(query.LastName));
            }

            if (query != null && !string.IsNullOrWhiteSpace(query.Email))
            {
                queryOver = queryOver.Where(() => userAlias.Email.IsInsensitiveLike(query.Email));
            }

            if (query != null && query.IsAbandoned)
            {
                queryOver = queryOver.Where(() => userAlias.Email != null);
            }

            return queryOver.TransformUsing(Transformers.AliasToBean<SessionDataQueryResult>());
        }

        private class UserCartItemCountQueryResult
        {
            public Guid UserGuid { get; set; }
            public int Count { get; set; }
        }

        private class SessionDataQueryResult
        {
            public Guid UserGuid { get; set; }
            public int? UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserEmail { get; set; }
            public string SessionEmail { get; set; }
            public string SessionShippingAddress { get; set; }
            public string SessionBillingAddress { get; set; }
            public bool BillingAddressIsSameAsShipping { get; set; }
            public DateTime UpdatedOn { get; set; }

            public string Email
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(UserEmail)
                        ? UserEmail
                        : (!string.IsNullOrWhiteSpace(SessionEmail)
                            ? JsonConvert.DeserializeObject<string>(SessionEmail)
                            : null);
                }
            }

            public Address ShippingAddress
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(SessionShippingAddress)
                        ? JsonConvert.DeserializeObject<Address>(SessionShippingAddress)
                        : null;
                }
            }

            public Address BillingAddress
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(SessionBillingAddress)
                        ? JsonConvert.DeserializeObject<Address>(SessionBillingAddress)
                        : null;
                }
            }
        }

    }
}