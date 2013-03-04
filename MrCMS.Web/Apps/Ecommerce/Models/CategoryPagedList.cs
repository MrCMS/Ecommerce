using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CategoryPagedList : StaticPagedList<Category>
    {
        public int? CategoryContainerId { get; private set; }

        public CategoryPagedList(IPagedList<Category> categories, int? categoryContainerId)
            : base(categories, categories)
        {
            CategoryContainerId = categoryContainerId;
        }
    }
}