using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Foolproof;
using Iesi.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers.Validation;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Web.Apps.Ecommerce.Entities.GiftCards;
using MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Helpers.Pricing;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Entities.Products
{
    public class ProductVariant : SiteEntity
    {
        public ProductVariant()
        {
            OptionValues = new List<ProductOptionValue>();
            PriceBreaks = new List<PriceBreak>();
            RequiresShipping = true;
            GoogleBaseProducts = new List<GoogleBaseProduct>();
            RestrictedTo = new HashSet<string>();
            WarehouseStock = new List<WarehouseStock>();
        }

        public virtual IList<WarehouseStock> WarehouseStock { get; set; }

        public virtual decimal Weight { get; set; }

        [StringLength(400)]
        public virtual string Name { get; set; }

        public virtual string EditUrl
        {
            get { return Product.EditUrl; }
        }

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
            get { return PreviousPrice.ProductPriceIncludingTax(TaxRatePercentage); }
        }

        public virtual decimal? PreviousPriceExcludingTax
        {
            get { return PreviousPrice.ProductPriceExcludingTax(TaxRatePercentage); }
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
                    ? ReducedBy/PreviousPrice.Value
                    : 0;
            }
        }

        public virtual decimal Price
        {
            get { return BasePrice.ProductPriceIncludingTax(TaxRatePercentage); }
        }

        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax
        {
            get { return BasePrice.ProductPriceExcludingTax(TaxRatePercentage); }
        }

        [Required]
        [Remote("IsUniqueSKU", "ProductVariant", AdditionalFields = "Id")]
        public virtual string SKU { get; set; }

        public virtual decimal Tax
        {
            get { return BasePrice.ProductTax(TaxRatePercentage); }
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

        [DisplayName("Stock Remaining")]
        public virtual int StockRemaining { get; set; }

        public virtual Product Product { get; set; }

        public virtual IList<ProductOptionValue> OptionValues { get; set; }

        public virtual IEnumerable<ProductOptionValue> AttributeValuesOrdered
        {
            get { return OptionValues.OrderBy(value => value.DisplayOrder); }
        }

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
                TaxRate taxRate = MrCMSApplication.Get<IGetDefaultTaxRate>().Get();
                return MrCMSApplication.Get<TaxSettings>().TaxesEnabled
                    ? TaxRate == null
                    ? taxRate != null ? taxRate.Percentage : decimal.Zero
                        : TaxRate.Percentage
                    : decimal.Zero;
            }
        }

        public virtual string DisplayName
        {
            get { return FullName; }
        }

        public virtual string FullName
        {
            get
            {
                var list = new List<string>();
                if (Product != null && !string.IsNullOrWhiteSpace(Product.Name))
                {
                    list.Add(Product.Name);
                }
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    if (Product != null && Product.Name != Name)
                    {
                        list.Add(Name);
                    }
                }
                if (OptionValues.Any())
                {
                    list.AddRange(AttributeValuesOrdered.Select(option => option.FormattedValue));
                }
                return string.Join(" - ", list);
            }
        }

        public virtual bool ShowPreviousPrice
        {
            get { return PreviousPrice.HasValue & PreviousPrice > Price; }
        }

        public virtual IList<GoogleBaseProduct> GoogleBaseProducts { get; set; }

        [DisplayName("Featured Image")]
        public virtual string FeaturedImageUrl { get; set; }

        [DisplayName("Sold Out")]
        public virtual bool SoldOut { get; set; }

        [DisplayName("Sold Out Message")]
        public virtual string SoldOutMessage { get; set; }

        //Gift Card Options
        [DisplayName("Is gift card?")]
        public virtual bool IsGiftCard { get; set; }

        [DisplayName("Gift card type")]
        public virtual GiftCardType GiftCardType { get; set; }

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
                    ? (IEnumerable<MediaFile>) Product.Images.OrderByDescending(file => file.FileUrl == DisplayImageUrl)
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

        [DisplayName("Has restricted shipping?")]
        public virtual bool HasRestrictedShipping { get; set; }

        [DisplayName("Restricted to?")]
        public virtual HashSet<string> RestrictedTo { get; set; }

        [DisplayName("Variant Type")]
        public virtual VariantType VariantType
        {
            get
            {
                return IsGiftCard
                    ? VariantType.GiftCard
                    : IsDownloadable
                        ? VariantType.Download
                        : VariantType.Standard;
            }
        }

        [DisplayName("Custom In Stock Message")]
        public virtual string CustomStockInStockMessage { get; set; }
        
        [DisplayName("Custom Out Of Stock Message")]
        public virtual string CustomStockOutOfStockMessage { get; set; }

        private PriceBreak GetPriceBreak(int quantity)
        {
            return PriceBreaks != null
                ? PriceBreaks.OrderBy(x => x.Price).FirstOrDefault(x => x.Quantity <= quantity)
                : null;
        }

        public virtual decimal GetUnitPrice(int quantity)
        {
            PriceBreak priceBreak = GetPriceBreak(quantity);
            return priceBreak != null
                ? priceBreak.PriceIncludingTax
                : Price;
        }

        public virtual string GetSelectOptionName(bool showName = true, bool showOptionValues = true)
        {
            string title = string.Empty;
            if (!string.IsNullOrWhiteSpace(Name) && showName)
                title = Name + " - ";

            if (OptionValues.Any() && showOptionValues)
            {
                title += string.Join(", ", AttributeValuesOrdered.Select(value => value.Value)) + " - ";
            }

            title += Price.ToCurrencyFormat();

            return title;
        }

        public virtual decimal Rating { get; set; }

        [DisplayName("Number of Reviews")]
        public virtual int NumberOfReviews { get; set; }

        [DisplayName("E-Tag")]
        public virtual ETag ETag { get; set; }

        public virtual bool CanShowEtag
        {
            get { return this.ETag != null && !string.IsNullOrWhiteSpace(this.ETag.Image); }
        }

        public virtual decimal GetSaving(int quantity)
        {
            return PreviousPriceIncludingTax.GetValueOrDefault() != 0
                ? ((PreviousPriceIncludingTax * quantity) - GetPrice(quantity)).Value
                : (Price * quantity) - GetPrice(quantity);
        }

    }
}