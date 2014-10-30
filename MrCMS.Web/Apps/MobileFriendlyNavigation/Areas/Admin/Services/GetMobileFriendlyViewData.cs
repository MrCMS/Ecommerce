using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Areas.Admin.Services
{
    public class GetMobileFriendlyViewData : BaseAssignWidgetAdminViewData<Widgets.MobileFriendlyNavigation>
    {
        private readonly ISession _session;

        public GetMobileFriendlyViewData(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Widgets.MobileFriendlyNavigation widget, ViewDataDictionary viewData)
        {
            viewData["rootNotes"] =
                _session.QueryOver<Webpage>().Where(x => x.Parent == null).OrderBy(webpage => webpage.DisplayOrder).Asc.List()
                    .BuildSelectItemList(webpage => webpage.Name, container => container.Id.ToString(), emptyItem: new SelectListItem{Value = "0", Text = "Root"});
        }
    }
}