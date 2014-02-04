using System;
using System.Collections.Generic;
using System.ComponentModel;
using Foolproof;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers.Validation;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductVariant : SiteEntity
    {
        public ProductVariant()
        {
            OptionValues = new List<ProductOptionValue>();
            PriceBreaks = new List<PriceBreak>();
            RestrictedShippingMethods = new List<ShippingMethod>();
            RequiresShipping = true;
        }

        public virtual decimal Weight { get; set; }
        [StringLength(400)]
        public virtual string Name { get; set; }
        public virtual string EditUrl { get { return Product.EditUrl; } }

        [Required]
        [DisplayName("Price")]
        [CurrencyValidator]
        [UIHint("Currency")]
        public virtual decimal BasePrice { get; set; }

        [DisplayName("Previous Price")]
        [CurrencyValidator]
        [UIHint("Currency")]
        public virtual decimal? PreviousPrice { get; set; }

        public virtual decimal? PreviousPriceIncludingTax
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(PreviousPrice, TaxRate); }
        }

        public virtual decimal? PreviousPriceExcludingTax
        {
            get { return TaxAwareProductPrice.GetPriceExcludingTax(PreviousPrice, TaxRate); }
        }

        public virtual decimal ReducedBy
        {
            get
            {
                return PreviousPrice != null
                           ? PreviousPrice.Value > PricePreTax
                                 ? PreviousPrice.Value - PricePreTax
                                 : 0
                           : 0;
            }
        }

        public virtual decimal ReducedByPercentage
        {
            get
            {
                return PreviousPrice != null && PreviousPrice != 0
                           ? ReducedBy / PreviousPrice.Value
                           : 0;
            }
        }

        public virtual decimal Price
        {
            get { return TaxAwareProductPrice.GetPriceIncludingTax(BasePrice, TaxRate); }
        }

        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax
        {
            get { return TaxAwareProductPrice.GetPriceExcludingTax(BasePrice, TaxRate); }
        }

        private PriceBreak GetPriceBreak(int quantity)
        {
            return PriceBreaks != null
                       ? PriceBreaks.OrderBy(x => x.Price).FirstOrDefault(x => x.Quantity <= quantity)
                       : null;
        }

        public virtual decimal GetPrice(int quantity)
        {
            return GetUnitPrice(quantity) * quantity;
        }

        public virtual decimal GetTax(int quantity)
        {
            return GetUnitTax(quantity) * quantity;
        }

        public virtual decimal GetUnitPrice(int quantity)
        {
            var priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                       ? priceBreak.PriceIncludingTax
                       : Price;
        }

        public virtual decimal GetUnitTax(int quantity)
        {
            var priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                       ? priceBreak.Tax
                       : Tax;
        }

        public virtual decimal GetUnitPricePreTax(int quantity)
        {
            return GetUnitPrice(quantity) - GetUnitTax(quantity);
        }

        public virtual decimal GetSaving(int quantity)
        {
            return PreviousPriceIncludingTax.GetValueOrDefault() != 0
                       ? ((PreviousPriceIncludingTax * quantity) - GetPrice(quantity)).Value
                       : (Price * quantity) - GetPrice(quantity);
        }

        [Required]
        [Remote("IsUniqueSKU", "ProductVariant", AdditionalFields = "Id")]
        public virtual string SKU { get; set; }

        public virtual decimal Tax { get { return TaxAwareProductPrice.GetTax(BasePrice, TaxRate); } }

        public virtual bool CanBuy(int quantity)
        {
            return quantity > 0 && (TrackingPolicy == TrackingPolicy.DontTrack || StockRemaining >= quantity);
        }

        public virtual ProductAvailability Availability
        {
            get
            {
                if (AvailableOn.HasValue && AvailableOn <= DateTime.UtcNow)
                    return ProductAvailability.Available;
                return ProductAvailability.PreOrder;
            }
        }

        [DisplayName("Available On")]
        public virtual DateTime? AvailableOn { get; set; }

        public virtual bool InStock
        {
            get { return TrackingPolicy == TrackingPolicy.DontTrack || StockRemaining > 0; }
        }

        [DisplayName("Stock Remaining")]
        public virtual int StockRemaining { get; set; }

        public virtual Product Product { get; set; }

        public virtual IList<ProductOptionValue> OptionValues { get; set; }
        public virtual IEnumerable<ProductOptionValue> AttributeValuesOrdered { get { return OptionValues.OrderBy(value => value.DisplayOrder); } }
        public virtual IList<PriceBreak> PriceBreaks { get; set; }

        [StringLength(200)]
        public virtual string Barcode { get; set; }
        [DisplayName("Tracking Policy")]
        public virtual TrackingPolicy TrackingPolicy { get; set; }

        [DisplayName("Tax Rate")]
        public virtual TaxRate TaxRate { get; set; }


        [DisplayName("Manufacturer Part Number")]
        [StringLength(250)]
        public virtual string ManufacturerPartNumber { get; set; }

        public virtual decimal TaxRatePercentage
        {
            get
            {
                return MrCMSApplication.Get<TaxSettings>().TaxesEnabled
                           ? TaxRate == null
                                 ? 0
                                 : TaxRate.Percentage
                           : 0;
            }
        }

        public virtual string DisplayName
        {
            get { return !string.IsNullOrWhiteSpace(Name) ? Name : (Product != null ? Product.Name : ""); }
        }
        public virtual string GetSelectOptionName(bool showName = true, bool showOptionValues = true)
        {
            var title = string.Empty;
            if (!string.IsNullOrWhiteSpace(Name) && showName)
                title = Name + " - ";

            if (OptionValues.Any() && showOptionValues)
            {
                title += string.Join(", ", AttributeValuesOrdered.Select(value => value.Value)) + " - ";
            }

            title += Price.ToCurrencyFormat();

            return title;
        }

        public virtual bool ShowPreviousPrice
        {
            get { return PreviousPrice.HasValue & PreviousPrice > Price; }
        }

        public virtual GoogleBaseProduct GoogleBaseProduct { get; set; }

        [DisplayName("Featured Image")]
        public virtual string FeaturedImageUrl { get; set; }

        [DisplayName("Sold Out")]
        public virtual bool SoldOut { get; set; }

        [DisplayName("Sold Out Message")]
        public virtual string SoldOutMessage { get; set; }

        [DisplayName("Allowed Shipping Methods")]
        public virtual IList<ShippingMethod> RestrictedShippingMethods { get; set; }

        //Download Options
        [DisplayName("Downloadable?")]
        public virtual bool IsDownloadable { get; set; }

        [DisplayName("Download File")]
        [RequiredIf("IsDownloadable", true)]
        public virtual string DownloadFileUrl { get; set; }

        [DisplayName("Demo File")]
        public virtual string DemoFileUrl { get; set; }

        [DisplayName("Allowed number of days for file download")]
        public virtual int? AllowedNumberOfDaysForDownload { get; set; }

        [DisplayName("Allowed number of downloads")]
        public virtual int? AllowedNumberOfDownloads { get; set; }

        public virtual string DisplayImageUrl
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FeaturedImageUrl)
                           ? FeaturedImageUrl
                           : Product != null
                                 ? Product.DisplayImageUrl
                                 : MrCMSApplication.Get<EcommerceSettings>().DefaultNoProductImage;
            }
        }
        public virtual IEnumerable<MediaFile> Images
        {
            get
            {
                return Product != null
                           ? (IEnumerable<MediaFile>)Product.Images.OrderByDescending(file => file.FileUrl == DisplayImageUrl)
                           : new List<MediaFile>();
            }
        }

        public virtual string DirectUrl
        {
            get
            {
                return Product == null
                           ? string.Empty
                           : string.Format("/{0}?variant={1}", Product.LiveUrlSegment, Id); 
            }
        }

        [DisplayName("Requires shipping?")]
        public virtual bool RequiresShipping { get; set; }

        public virtual CanBuyStatus CanBuy(CartModel cart, int additionalQuantity = 0)
        {
            if (!InStock)
                return new OutOfStock(this);
            var requestedQuantity = additionalQuantity;
            var existingItem = cart.Items.FirstOrDefault(item => item.Item == this);
            if (existingItem != null)
                requestedQuantity += existingItem.Quantity;
            if (TrackingPolicy == TrackingPolicy.Track && requestedQuantity > StockRemaining)
                return new CannotOrderQuantity(this, requestedQuantity);
            if (!cart.AvailableShippingMethods.Except(RestrictedShippingMethods).Any())
                return new NoShippingMethodWouldBeAvailable(this);
            return new CanBuy();
        }
    }

    public class NoShippingMethodWouldBeAvailable : CanBuyStatus
    {
        private readonly ProductVariant _variant;

        public NoShippingMethodWouldBeAvailable(ProductVariant variant)
        {
            _variant = variant;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get { return string.Format("You cannot order {0} as adding it to your cart would mean that there are no availble shipping methods", _variant.DisplayName); }
        }
    }

    public abstract class CanBuyStatus
    {
        public abstract bool OK { get; }
        public abstract string Message { get; }
    }

    public class CanBuy : CanBuyStatus
    {
        public override bool OK
        {
            get { return true; }
        }

        public override string Message
        {
            get { return null; }
        }
    }

    public class OutOfStock : CanBuyStatus
    {
        private readonly ProductVariant _variant;

        public OutOfStock(ProductVariant variant)
        {
            _variant = variant;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get { return string.Format("Sorry, but {0} is currently out of stock", _variant.DisplayName); }
        }
    }

    public class CannotOrderQuantity : CanBuyStatus
    {
        private readonly ProductVariant _variant;
        private readonly int _requestedQuantity;

        public CannotOrderQuantity(ProductVariant variant, int requestedQuantity)
        {
            _variant = variant;
            _requestedQuantity = requestedQuantity;
        }

        public override bool OK
        {
            get { return false; }
        }

        public override string Message
        {
            get
            {
                return string.Format("Sorry, but there are currently only {0} units of {1} in stock",
                                     _variant.StockRemaining, _variant.DisplayName);
            }
        }
    }
}