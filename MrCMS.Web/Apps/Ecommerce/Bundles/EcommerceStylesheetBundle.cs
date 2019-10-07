using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Ecommerce.Bundles
{
    public class EcommerceStylesheetBundle : IStylesheetBundle
    {
        public const string BundleUrl = "~/ecommerce/stylesheets";

        public string Url 
        {
            get
            {
                return BundleUrl;
            }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Apps/Ecommerce/Content/bootstrap/css/bootstrap.css";
                yield return "~/Apps/Ecommerce/Content/bootstrap/css/bootstrap-theme.css";
                yield return "~/Apps/Ecommerce/Content/FontAwesome/css/font-awesome.css";
                yield return "~/Apps/Ecommerce/Content/Styles/jquery-ui-bootstrap/jquery-ui-1.9.2.custom.css";
                yield return "~/Apps/Ecommerce/Content/Styles/prettyPhoto.css";
                yield return "~/Apps/Ecommerce/Content/Styles/style.css";
                yield return "~/Apps/Ecommerce/Content/Styles/custom/typeahead-custom-template.css";
            }
        }
    }
}