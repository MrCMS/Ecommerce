using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public class GetValidDiscounts : IGetValidDiscounts
    {
        private readonly ISession _session;

        public GetValidDiscounts(ISession session)
        {
            _session = session;
        }

        public IList<Discount> Get(CartModel cart, List<string> discountCodes)
        {
            DateTime now = CurrentRequestData.Now;
            return _session.QueryOver<Discount>()
                 .Where(
                     discount =>
                         (discount.Code.IsIn(discountCodes) || !discount.RequiresCode) &&
                         (discount.ValidFrom == null || discount.ValidFrom <= now) &&
                         (discount.ValidUntil == null || discount.ValidUntil >= now))
                 .Cacheable().List();
        }
    }
}