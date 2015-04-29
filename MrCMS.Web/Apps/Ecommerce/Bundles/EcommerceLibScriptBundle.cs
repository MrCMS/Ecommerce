using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Ecommerce.Bundles
{
    public class EcommerceLibScriptBundle : IScriptBundle
    {
        public string Url
        {
            get { return "~/ecommerce/scripts/lib"; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Apps/Ecommerce/Content/bootstrap/js/bootstrap.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.unobtrusive.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/ecommerce.js";
            }
        }
    }
}