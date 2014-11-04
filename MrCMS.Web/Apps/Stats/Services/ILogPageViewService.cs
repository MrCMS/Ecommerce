using MrCMS.Web.Apps.Stats.Controllers;

namespace MrCMS.Web.Apps.Stats.Services
{
    public interface ILogPageViewService
    {
        void LogPageView(PageViewInfo info);
    }
}