using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Ecommerce.Bundles
{
    public class CheckoutStylesheetBundle : IStylesheetBundle
    {
        public const string BundleUrl = "~/checkout/stylesheets";

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
                yield return "~/Apps/Ecommerce/Content/Styles/jquery-ui-bootstrap/jquery-ui-1.9.2.custom.css";
                yield return "~/Apps/Ecommerce/Content/Styles/checkout.css";
                yield return "~/Apps/Ecommerce/Content/Styles/custom/typeahead-custom-template.css";
            }
        }
    }
}