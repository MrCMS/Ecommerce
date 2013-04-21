using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using NHibernate;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Paging;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class BrandService : IBrandService
    {
        private readonly ISession _session;

        public BrandService(ISession session)
        {
            _session = session;
        }

        public IList<Brand> GetAll()
        {
            return _session.QueryOver<Brand>().Cacheable().List();
        }

        public IPagedList<Brand> GetPaged(int pageNum, string search, int pageSize = 10)
        {
            return BaseQuery(search).Paged(pageNum, pageSize);
        }

        private IQueryOver<Brand, Brand> BaseQuery(string search)
        {
            if (String.IsNullOrWhiteSpace(search))
                return
                _session.QueryOver<Brand>()
                        .OrderBy(entry => entry.Name).Asc;
            else
                return
                  _session.QueryOver<Brand>().Where(x => x.Name == search)
                          .OrderBy(entry => entry.Name).Asc;
        }

        public void Add(Brand brand)
        {
            _session.Transact(session => session.Save(brand));
        }

        public void Update(Brand brand)
        {
            _session.Transact(session => session.Update(brand));
        }

        public void Delete(Brand brand)
        {
            _session.Transact(session => session.Delete(brand));
        }
    }
}