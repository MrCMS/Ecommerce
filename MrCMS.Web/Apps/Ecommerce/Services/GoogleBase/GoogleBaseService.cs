using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Helpers;
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

        public GoogleBaseProduct GetGoogleBaseProduct(int id)
        {
            return _session.QueryOver<GoogleBaseProduct>().Where(x => x.Id == id).SingleOrDefault();
        }

        public void SaveGoogleBaseProduct(GoogleBaseProduct item)
        {
            if (item.ProductVariant != null)
                item.ProductVariant.GoogleBaseProduct = item;
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public List<SelectListItem> GetGoogleCategories()
        {
            return GoogleBaseTaxonomyData.GetCategories().BuildSelectItemList(item => item.Name, item => item.Name, emptyItem: null); ;
        }

        public IPagedList<GoogleBaseCategory> Search(string queryTerm = null, int page = 1)
        {
            var categories = GoogleBaseTaxonomyData.GetCategories();

            return !string.IsNullOrWhiteSpace(queryTerm) ? categories.Where(x => x.Name.ToLower().Contains(queryTerm.ToLower())).Paged(page, 10) : categories.Paged(page, 10);
        }
    }
}