using MrCMS.Entities;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class Brand : SiteEntity
    {
        [Remote("IsUniqueName", "Brand", AdditionalFields = "Id")]
        public virtual string Name { get; set; }
    }
}