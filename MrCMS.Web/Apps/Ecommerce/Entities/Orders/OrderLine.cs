using System;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
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

        public virtual decimal Subtotal { get { return Quantity * UnitPrice; } }

        public virtual bool IsDownloadable { get; set; }
        public virtual int? AllowedNumberOfDownloads { get; set; }
        public virtual DateTime? DownloadExpiresOn { get; set; }
        public virtual int NumberOfDownloads { get; set; }
        public virtual string DownloadFileUrl { get; set; }
        public virtual string DownloadFileContentType { get; set; }
        public virtual string DownloadFileName { get; set; }
        public virtual string DownloadMaskedLink { get { return string.Format("http://{0}/digital-download/{1}/{2}", Site.BaseUrl, Order.Guid, Id); } }

    }
}