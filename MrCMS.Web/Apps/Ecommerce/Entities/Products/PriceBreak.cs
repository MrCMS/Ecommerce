using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class PriceBreak : SiteEntity
    {
        public virtual IBuyableItem Item { get; set; }
        public virtual int Quantity { get; set; }
        public virtual decimal Price { get; set; }
    }
}