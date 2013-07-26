using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.GoogleBase
{
    public class GoogleBaseTaxonomyService : IGoogleBaseTaxonomyService
    {
        private static List<string> _googleBaseTaxonomy;
        private static IEnumerable<string> GoogleBaseTaxonomyItems
        {
            get
            {
                var rows = GoogleBaseTaxonomyData.RawData.Split(new[] { "\n","\r" }, StringSplitOptions.RemoveEmptyEntries);
                return _googleBaseTaxonomy ?? (_googleBaseTaxonomy = rows.Select(s => s).ToList());
            }
        }

        public List<SelectListItem> GetOptions()
        {
            return GoogleBaseTaxonomyItems.BuildSelectItemList(item => item, item => item,emptyItem: null);
        }
    }
}