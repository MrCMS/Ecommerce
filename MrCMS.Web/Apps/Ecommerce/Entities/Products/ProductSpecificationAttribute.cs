using System.Collections.Generic;
using System.ComponentModel;
using Iesi.Collections.Generic;
using MrCMS.Entities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductSpecificationAttribute : SiteEntity
    {
        public ProductSpecificationAttribute()
        {
            Options = new List<ProductSpecificationAttributeOption>();
            HiddenInCategories = new HashedSet<Category>();
        }

        [Required]
        [Remote("IsUniqueAttribute", "ProductSpecificationAttribute", AdditionalFields = "Id")]
        public virtual string Name { get; set; }
        public virtual int DisplayOrder { get; set; }
        [DisplayName("Hide in Search")]
        public virtual bool HideInSearch { get; set; }

        public virtual IList<ProductSpecificationAttributeOption> Options { get; set; }
        public virtual Iesi.Collections.Generic.ISet<Category> HiddenInCategories { get; set; }
    }
}