using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductPagedList : StaticPagedList<Product>
    {
        public int? ProductContainerId { get; private set; }

        public ProductPagedList(IPagedList<Product> products, int? productContainerId)
            : base(products, products)
        {
            ProductContainerId = productContainerId;
        }
    }
}