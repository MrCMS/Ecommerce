using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ETagAdminService : IETagAdminService
    {
        private readonly ISession _session;

        public ETagAdminService(ISession session)
        {
            _session = session;
        }

        public IPagedList<ETag> Search(ETagSearchQuery query)
        {
            IQueryOver<ETag, ETag> queryOver = _session.QueryOver<ETag>();

            if (!string.IsNullOrWhiteSpace(query.Name))
                queryOver = queryOver.Where(eTag => eTag.Name.IsInsensitiveLike(query.Name, MatchMode.Anywhere));

            return queryOver.OrderBy(eTag => eTag.Name).Asc.Paged(query.Page);
        }

        public void Add(ETag eTag)
        {
            _session.Transact(session => session.Save(eTag));
        }

        public void Update(ETag eTag)
        {
            _session.Transact(session => session.Update(eTag));
        }

        public void Delete(ETag eTag)
        {
            _session.Transact(session => session.Delete(eTag));
        }

        public List<SelectListItem> GetOptions()
        {
            return GetAll()
                .OrderBy(x => x.Name)
                .BuildSelectItemList(item => item.Name, item => item.Id.ToString(), emptyItem: new SelectListItem
                {
                    Text = "Please select...",
                    Value = "0"
                });
        }

        public IList<ETag> GetAll()
        {
            return _session.QueryOver<ETag>().OrderBy(x => x.Name).Asc.Cacheable().List();
        }

        public ETag GetById(int id)
        {
            return _session.QueryOver<ETag>().Where(x => x.Id == id).SingleOrDefault();
        }

        public ETag GetETagByName(string name)
        {
            return _session.QueryOver<ETag>()
                            .Where(
                                brand =>
                                brand.Name.IsInsensitiveLike(name, MatchMode.Exact)).SingleOrDefault();
        }

        public bool NameIsValidForETag(string name, int? id)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            var query = _session.QueryOver<ETag>()
                .Where(eTag => eTag.Name == name);

            if (id.HasValue)
                query = query.Where(x => x.Id != id);
            
            return query.Cacheable()
                .RowCount() > 0;
        }
    }
}