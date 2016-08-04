using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductVariantPagedList : StaticPagedList<ProductVariant>
    {
        public int? ProductContainerId { get; private set; }

        public ProductVariantPagedList(IPagedList<ProductVariant> products, int? productContainerId)
            : base(products, products)
        {
            ProductContainerId = productContainerId;
        }
    }
}