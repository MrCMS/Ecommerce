using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using System;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Orders
{
    public class OrderLine : SiteEntity
    {
        public virtual Order Order { get; set; }
        [DisplayName("Product")]
        public virtual ICanAddToCart ProductVariant { get; set; }
        public virtual int Quantity { get; set; }
        [DisplayName("Unit Price")]
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal Subtotal { get; set; }
        public virtual decimal Tax { get; set; }
        [DisplayName("Tax Rate")]
        public virtual decimal TaxRate { get; set; }
        public virtual decimal Discount { get; set; }
        public virtual decimal Weight { get; set; }
    }
}