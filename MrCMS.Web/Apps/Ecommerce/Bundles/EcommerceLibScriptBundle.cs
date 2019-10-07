using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Ecommerce.Bundles
{
    public class EcommerceLibScriptBundle : IScriptBundle
    {
        public const string BundleUrl = "~/ecommerce/scripts";

        public string Url
        {
            get { return BundleUrl; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Scripts/jquery-1.10.2.js";

                yield return "~/Apps/Ecommerce/Content/bootstrap/js/bootstrap.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.unobtrusive.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/ecommerce.js";


                yield return "~/Scripts/handlebars.js";
                yield return "~/Scripts/typeahead.bundle.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/typeahead-custom-templates.js";
            }
        }
    }
}