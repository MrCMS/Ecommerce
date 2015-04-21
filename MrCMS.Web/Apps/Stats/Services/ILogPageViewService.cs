using MrCMS.Web.Apps.Stats.Controllers;
using MrCMS.Web.Apps.Stats.Models;

namespace MrCMS.Web.Apps.Stats.Services
{
    public interface ILogPageViewService
    {
        void LogPageView(PageViewInfo info);
    }
}