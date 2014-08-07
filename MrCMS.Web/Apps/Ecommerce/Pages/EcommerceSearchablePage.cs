using Iesi.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public abstract class EcommerceSearchablePage : Webpage
    {
        protected EcommerceSearchablePage()
        {
            HiddenSearchSpecifications = new HashedSet<ProductSpecificationAttribute>();
        }

        public virtual ISet<ProductSpecificationAttribute> HiddenSearchSpecifications { get; set; }
    }
}