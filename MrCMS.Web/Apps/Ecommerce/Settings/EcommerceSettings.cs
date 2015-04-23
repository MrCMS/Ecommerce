using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class EcommerceSettings : SiteSettingsBase
    {
        public EcommerceSettings()
        {
            DashboardRevenueDays = 7;
            TermsAndConditionsRequired = true;
            DefaultProductSearchSort = ProductSearchSort.MostPopular;
            GiftMessageMaxLength = 250;

            EncryptionPassPhrase = "MrCMS Ecommerce's passphrase for session encryption and decryption";
            DefaultSessionExpiryDays = 28;
        }


        [DisplayName("Search Products per Page")]
        public string SearchProductsPerPage { get; set; }

        [DisplayName("Previous Price text")]
        public string PreviousPriceText { get; set; }

        [DisplayName("Product Image not specified replacement image")]
        public string DefaultNoProductImage { get; set; }

        [DisplayName("PDF Invoice Logo Image")]
        public string ReportLogoImage { get; set; }

        [DisplayName("PDF Invoice Footer Text")]
        public string ReportFooterText { get; set; }

        [DisplayName("Enable Wish Lists")]
        public bool EnableWishlists { get; set; }

        [DisplayName("Enable Gift Cards")]
        public bool GiftCardsEnabled { get; set; }

        [DisplayName("Enable Reward Points")]
        public bool RewardPointsEnabled { get; set; }

        [DisplayName("Enable Gift Message")]
        public bool GiftMessageEnabled { get; set; }

        [DisplayName("Max Gift Message Length")]
        public int GiftMessageMaxLength { get; set; }

        [DisplayName("Enable warehouse-based stock management")]
        public bool WarehouseStockEnabled { get; set; }

        [DisplayName("Enable terms and conditions acceptance")]
        public bool TermsAndConditionsRequired { get; set; }

        public IEnumerable<int> ProductPerPageOptions
        {
            get
            {
                return (SearchProductsPerPage ?? string.Empty).Split(',').Where(s =>
                {
                    int result;
                    return int.TryParse(s, out result);
                }).Select(s => Convert.ToInt32(s));
            }
        }

        public IEnumerable<SelectListItem> ProductPerPageOptionItems
        {
            get
            {
                return ProductPerPageOptions.BuildSelectItemList(i => string.Format("{0} products per page", i),
                    i => i.ToString(),
                    emptyItem: null);
            }
        }

        public override bool RenderInSettings
        {
            get { return false; }
        }

        [DisplayName("Site Currency")]
        public int CurrencyId { get; set; }

        [DisplayName("X Days to be shown in Dashboard Revenue")]
        public int DashboardRevenueDays { get; set; }

        [DisplayName("Default Product Search Sort")]
        public ProductSearchSort DefaultProductSearchSort { get; set; }

        public string EncryptionPassPhrase { get; set; }
        public int DefaultSessionExpiryDays { get; set; }

        [DisplayName("Product Url")]
        public string ProductUrl { get; set; }

        [DisplayName("Brand Url")]
        public string BrandUrl { get; set; }

        [DisplayName("Category Url")]
        public string CategoryUrl { get; set; }
    }
}