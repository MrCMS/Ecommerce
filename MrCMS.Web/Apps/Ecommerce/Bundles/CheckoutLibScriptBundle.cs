using System.Collections.Generic;
using MrCMS.Website.Optimization;

namespace MrCMS.Web.Apps.Ecommerce.Bundles
{
    public class CheckoutLibScriptBundle : IScriptBundle
    {
        public const string BundleUrl = "~/checkout/scripts";

        public string Url
        {
            get { return BundleUrl; }
        }

        public IEnumerable<string> Files
        {
            get
            {
                yield return "~/Apps/Ecommerce/Content/bootstrap/js/bootstrap.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/jquery.validate.unobtrusive.min.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/ecommerce.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/checkout.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/discount.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/gift-message.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/gift-cards.js";
                yield return "~/Apps/Ecommerce/Content/Scripts/custom/reward-points.js";                        
                
            }
        }
    }
}