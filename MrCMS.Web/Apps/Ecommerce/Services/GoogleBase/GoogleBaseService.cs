using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseService : IGoogleBaseService
    {
        private readonly ISession _session;

        public GoogleBaseService(ISession session)
        {
            _session = session;
        }

        public GoogleBaseProduct Get(int id)
        {
            return _session.QueryOver<GoogleBaseProduct>().Where(x=>x.Id==id).SingleOrDefault();
        }

        public void Add(GoogleBaseProduct item)
        {
            _session.Transact(session => session.Save(item));
        }

        public void Update(GoogleBaseProduct item)
        {
            _session.Transact(session => session.Update(item));
        }

        public List<SelectListItem> GetGoogleCategories()
        {
            return GoogleBaseTaxonomyData.Rows.BuildSelectItemList(item => item, item => item, emptyItem: null);
        }
    }
}