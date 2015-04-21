using MrCMS.Services.Widgets;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Services
{
    public class GetMobileFriendlyNavigationNodes:GetWidgetModelBase<Widgets.MobileFriendlyNavigation>
    {
        private readonly IMobileFriendlyNavigationService _mobileFriendlyNavigationService;

        public GetMobileFriendlyNavigationNodes(IMobileFriendlyNavigationService mobileFriendlyNavigationService)
        {
            _mobileFriendlyNavigationService = mobileFriendlyNavigationService;
        }

        public override object GetModel(Widgets.MobileFriendlyNavigation widget)
        {
            return _mobileFriendlyNavigationService.GetRootNodes(widget.RootWebpage);
        }
    }
}