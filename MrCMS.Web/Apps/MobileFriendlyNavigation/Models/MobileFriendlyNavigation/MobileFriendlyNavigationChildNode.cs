using System;
using System.Web;

namespace MrCMS.Web.Apps.MobileFriendlyNavigation.Models.MobileFriendlyNavigation
{
    public class MobileFriendlyNavigationChildNode
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public int ChildCount { get; set; }
        public DateTime? PublishOn { get; set; }
        public string DocumentType { get; set; }
        public int DisplayOrder { get; set; }

        public bool HasSubMenu
        {
            get { return ChildCount > 0; }
        }

        public HtmlString Url
        {
            get { return new HtmlString((!UrlSegment.StartsWith("/") ? "/" : string.Empty) + UrlSegment); }
        }


        public string GetJsHasSubMenu()
        {
            return (ChildCount > 0).ToString().ToLower();
        }

        public object ToJson()
        {
            return new
            {
                id = Id,
                text = Name,
                url = Url.ToString(),
                hasChildren = HasSubMenu
            };
        }
    }
}