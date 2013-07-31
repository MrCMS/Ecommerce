using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseTaxonomyService : IGoogleBaseTaxonomyService
    {
        public List<SelectListItem> GetOptions()
        {
            return GoogleBaseTaxonomyData.Rows.BuildSelectItemList(item => item, item => item, emptyItem: null);
        }
    }
}