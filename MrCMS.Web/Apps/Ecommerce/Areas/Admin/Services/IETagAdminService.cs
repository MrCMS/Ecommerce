using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using NHibernate.Mapping;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface IETagAdminService
    {
        IPagedList<ETag> Search(ETagSearchQuery searchQuery);
        void Add(ETag eTag);
        void Update(ETag eTag);
        void Delete(ETag eTag);
        List<SelectListItem> GetOptions();

        ETag GetById(int id);
        ETag GetETagByName(string name);
    }
}