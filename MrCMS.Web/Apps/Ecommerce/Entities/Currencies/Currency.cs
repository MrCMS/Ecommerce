using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Currencies
{
    public class Currency : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        [StringLength(3, MinimumLength = 3)]
        public virtual string Code { get; set; }
        [Required]
        public virtual string Format { get; set; }

        public virtual string FormatPrice(decimal price)
        {
            return price.ToString(Format);
        }
    }
}