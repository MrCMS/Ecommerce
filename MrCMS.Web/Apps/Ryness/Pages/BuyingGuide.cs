using System.Collections.Generic;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ryness.Entities;

namespace MrCMS.Web.Apps.Ryness.Pages
{
    public class BuyingGuide : TextPage
    {
        public virtual IList<BuyingGuideInformation> SubSections { get; set; }
    }
}