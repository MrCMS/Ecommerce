using MrCMS.Entities;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
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
        //product title or product variant name if available
        public virtual string Name { get; set; }
        //ie blue large cotton
        public virtual string Options { get; set; }
        public virtual string SKU { get; set; }
        public virtual Order Order { get; set; }
        [DisplayName("Product")]
        public virtual ProductVariant ProductVariant { get; set; }
        public virtual int Quantity { get; set; }
        [DisplayName("Unit Price")]
        public virtual decimal UnitPrice { get; set; }
        public virtual decimal UnitPricePreTax { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal PricePreTax { get; set; }
        public virtual decimal Tax { get; set; }
        [DisplayName("Tax Rate")]
        public virtual decimal TaxRate { get; set; }
        public virtual decimal Discount { get; set; }
        public virtual decimal Weight { get; set; }
    }
}