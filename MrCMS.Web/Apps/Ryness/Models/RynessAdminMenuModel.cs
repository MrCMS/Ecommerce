using System.Collections.Generic;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Ryness.Models
{
    public class RynessAdminMenuModel : IAdminMenuItem
    {
        private Dictionary<string, List<IMenuItem>> _children;
        public string Text { get { return "Ryness"; } }
        public string Url { get; private set; }
        public bool CanShow { get; private set; }
        public IDictionary<string, List<IMenuItem>> Children
        {
            get
            {
                return _children ?? 
                    (_children = new Dictionary<string, List<IMenuItem>>
                    {
                        {
                            "",
                            new List<IMenuItem>
                                {
                                    new ChildMenuItem("Testimonials", "/Admin/Apps/Ryness/Testimonial")
                                }
                        }
                    });
            }
        }
        public int DisplayOrder { get { return 49; } }
    }
}