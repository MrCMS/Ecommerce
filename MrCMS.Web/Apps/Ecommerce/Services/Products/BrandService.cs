using System.Collections.Generic;
using NHibernate;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Paging;
using System;
using NHibernate.Criterion;
using System.Web.Mvc;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public class BrandService : IBrandService
    {
        private readonly ISession _session;
        private readonly IProductSearchService _productSearchService;

        public BrandService(ISession session, IProductSearchService productSearchService)
        {
            _session = session;
            _productSearchService = productSearchService;
        }

        public Brand GetBrandByName(string name)
        {
            return _session.QueryOver<Brand>()
                            .Where(
                                brand =>
                                brand.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }
        public Brand GetById(int id)
        {
            return _session.QueryOver<Brand>().Where(x =>x.Id==id).SingleOrDefault();
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
                  _session.QueryOver<Brand>().Where(x => x.Name.IsInsensitiveLike(search, MatchMode.Anywhere))
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

        public bool AnyExistingBrandsWithName(string name, int id)
        {
            return _session.QueryOver<Brand>()
                           .Where(
                               option =>
                               option.Name.IsInsensitiveLike(name, MatchMode.Exact) && option.Id != id)
                           .RowCount() > 0;
        }
        public List<SelectListItem> GetOptions()
        {
            return GetAll().OrderBy(x=>x.Name).BuildSelectItemList(item => item.Name, item => item.Id.ToString(), null, new SelectListItem()
            {
                Text = "Please select...",
                Value = "0"
            });
        }

        public List<SelectListItem> GetAvailableBrands(ProductSearchQuery query)
        {
            var brands = _productSearchService.GetBrands(query);
            var items = GetAll().Where(item => brands.Contains(item.Id));
            return items.BuildSelectItemList(brand => brand.Name, brand => brand.Id.ToString(),
                                             brand => brand.Id == query.BrandId, "All Brands");
        }
    }
}