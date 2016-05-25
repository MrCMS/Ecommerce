using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Discounts
{
    public class Discount : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }

        [DisplayName("Requires Code?")]
        public virtual bool RequiresCode { get; set; }

        [StringLength(30, ErrorMessage = "Minimum length for code is {2} characters.", MinimumLength = 3)]
        public virtual string Code { get; set; }

        public virtual IList<DiscountUsage> DiscountUsages { get; set; }

        [DisplayName("Valid From")]
        public virtual DateTime? ValidFrom { get; set; }

        [DisplayName("Valid Until")]
        public virtual DateTime? ValidUntil { get; set; }

        public virtual IList<DiscountLimitation> Limitations { get; set; }
        public virtual IList<DiscountApplication> Applications { get; set; }

        public virtual bool CanBeAppliedFromUrl { get; set; }
        public virtual string RedirectUrl { get; set; }
        public virtual string Message { get; set; }
    }
}