using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class DiscountAdminService : IDiscountAdminService
    {
        private readonly ISession _session;

        public DiscountAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Discount> Search(DiscountSearchQuery query)
        {
            IQueryOver<Discount, Discount> queryOver = _session.QueryOver<Discount>();

            if (!string.IsNullOrWhiteSpace(query.DiscountCode))
                queryOver =
                    queryOver.Where(discount => discount.Code.IsInsensitiveLike(query.DiscountCode, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(query.Name))
                queryOver = queryOver.Where(discount => discount.Name.IsInsensitiveLike(query.Name, MatchMode.Anywhere));

            DateTime now = CurrentRequestData.Now;
            if (!query.ShowExpired)
                queryOver = queryOver.Where(discount => discount.ValidUntil == null || discount.ValidUntil >= now);

            return queryOver.OrderBy(discount => discount.Name).Asc.Paged(query.Page);
        }

        public void Add(Discount discount)
        {
            _session.Transact(session => session.Save(discount));
        }

        public void Update(Discount discount)
        {
            _session.Transact(session => session.Update(discount));
        }

        public void Delete(Discount discount)
        {
            _session.Transact(session => session.Delete(discount));
        }

        public IList<DiscountUsage> GetUsages(Discount discount)
        {
            return _session.QueryOver<DiscountUsage>()
                .Where(usage => usage.Discount.Id == discount.Id)
                .Fetch(usage => usage.Order).Eager
                .OrderBy(x => x.CreatedOn).Desc
                .Cacheable().List();
        }
    }
}