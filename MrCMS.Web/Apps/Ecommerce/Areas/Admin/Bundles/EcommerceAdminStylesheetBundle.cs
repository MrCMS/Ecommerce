using System.Collections.Generic;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Bundles
{
    public class EcommerceAdminStylesheetBundle : IAppStylesheetList
    {
        public IEnumerable<string> UIStylesheets
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<string> AdminStylesheets
        {
            get { yield return @"~/Apps/Ecommerce/Areas/Admin/Content/Styles/ecommerce-admin.css"; }
        }
    }
}