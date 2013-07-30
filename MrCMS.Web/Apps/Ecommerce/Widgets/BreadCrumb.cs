using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Widgets
{
    public class BreadCrumb : Widget
    {
        public override object GetModel(NHibernate.ISession session)
        {
            List<Webpage> pages = CurrentRequestData.CurrentPage.ActivePages.ToList();

            var currentPage = CurrentRequestData.CurrentPage;

            if (currentPage is Product)
            {
                var categories = (currentPage as Product).Categories;
                if (categories.Any())
                {
                    var mainCategory = categories.First();
                    pages = GetCategoryTree(mainCategory).ToList();
                    List<Webpage> pageList = pages.ToList();
                    pageList.Reverse();
                    pageList.Add(currentPage);
                    return pageList.AsEnumerable();
                }
            }
            pages.Reverse();
            return pages;
        }

        protected virtual IEnumerable<Webpage> GetCategoryTree(Webpage category)
        {
            var page = category;
            while (page != null)
            {
                yield return page;
                page = page.Parent.Unproxy() as Webpage;
            }
        }
    }
}