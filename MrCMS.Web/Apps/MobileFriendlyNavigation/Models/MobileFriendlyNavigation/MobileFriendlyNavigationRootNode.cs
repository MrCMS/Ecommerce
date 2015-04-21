using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationRootNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public string DocumentType { get; set; }
        public int DisplayOrder { get; set; }

        public List<MobileFriendlyNavigationChildNode> Children { get; set; }

        public bool HasSubMenu
        {
            get { return Children.Any(); }
        }

        public HtmlString Url
        {
            get { return new HtmlString((!UrlSegment.StartsWith("/") ? "/" : string.Empty) + UrlSegment); }
        }

        public HtmlString Text
        {
            get { return new HtmlString(Name); }
        }

        public string ChildrenAsJson()
        {
            return Json.Encode(Children.Select(x => x.ToJson()));
        }
    }
}