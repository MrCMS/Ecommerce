using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CategoryPagedList : StaticPagedList<Category>
    {
        public CategoryPagedList(IPagedList<Category> categories)
            : base(categories, categories)
        {
        }
    }
}