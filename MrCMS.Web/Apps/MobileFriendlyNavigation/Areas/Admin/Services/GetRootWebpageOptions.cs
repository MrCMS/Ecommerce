using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Areas.Admin.Services
{
    public class GetRootWebpageOptions : BaseAssignWidgetAdminViewData<Widgets.MobileFriendlyNavigation>
    {
        private readonly ISession _session;

        public GetRootWebpageOptions(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Widgets.MobileFriendlyNavigation widget, ViewDataDictionary viewData)
        {
            var webpages =
                _session.QueryOver<Webpage>()
                    .Where(webpage => webpage.Parent == null)
                    .OrderBy(webpage => webpage.DisplayOrder)
                    .Asc.List();

            viewData["root-webpage-options"] = webpages.BuildSelectItemList(webpage => webpage.Name, webpage => webpage.Id.ToString(),
                emptyItem: SelectListItemHelper.EmptyItem("Site Root", "0"));
        }
    }
}