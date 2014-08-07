using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Widgets;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Widgets;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Services
{
    public class GetBreadCrumb : GetWidgetModelBase<BreadCrumb>
    {
        public override object GetModel(BreadCrumb widget)
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