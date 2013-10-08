using System.ComponentModel;
using MrCMS.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Entities.GoogleBase
{
    public class GoogleBaseProduct : SiteEntity
    {
        public virtual ProductVariant ProductVariant { get; set; }

        //Override Defaults
        [DisplayName("Please choose Category")]
        public virtual string OverrideCategory { get; set; }
        public virtual string Category
        {
            get
            {
                return string.IsNullOrWhiteSpace(OverrideCategory) ? MrCMSApplication.Get<GoogleBaseSettings>().DefaultCategory : OverrideCategory;
            }
            set { OverrideCategory = value; }
        }
        [DisplayName("Product Condition")]
        public virtual ProductCondition? OverrideCondition { get; set; }
        public virtual ProductCondition Condition
        {
            get
            {
                return OverrideCondition.HasValue ? OverrideCondition.Value : MrCMSApplication.Get<GoogleBaseSettings>().DefaultCondition;
            }
            set { OverrideCondition = value; }
        }

        //Clothing & Accessories
        public virtual Gender Gender { get; set; }
        [DisplayName("Age Group")]
        public virtual AgeGroup AgeGroup { get; set; }

        //Attributes
        public virtual string Material
        {
            get
            {
                if (ProductVariant.OptionValues != null && ProductVariant.OptionValues.Any())
                {
                    var optionValue = ProductVariant.OptionValues.SingleOrDefault(x => x.ProductOption.Name == "Material");
                    if (optionValue != null)
                        return optionValue.Value;
                }
                return string.Empty;
            }
        }
        public virtual string Pattern
        {
            get
            {
                if (ProductVariant.OptionValues != null && ProductVariant.OptionValues.Any())
                {
                    var optionValue = ProductVariant.OptionValues.SingleOrDefault(x => x.ProductOption.Name == "Pattern");
                    if (optionValue != null)
                        return optionValue.Value;
                }
                return string.Empty;
            }
        }
        public virtual string Color
        {
            get
            {
                if (ProductVariant.OptionValues != null && ProductVariant.OptionValues.Any())
                {
                    var optionValue = ProductVariant.OptionValues.SingleOrDefault(x => x.ProductOption.Name == "Color" || x.ProductOption.Name == "Colour");
                    if (optionValue != null)
                        return optionValue.Value;
                }
                return string.Empty;
            }
        }
        public virtual string Size
        {
            get
            {
                if (ProductVariant.OptionValues != null && ProductVariant.OptionValues.Any())
                {
                    var optionValue = ProductVariant.OptionValues.SingleOrDefault(x => x.ProductOption.Name == "Size");
                    if (optionValue != null)
                        return optionValue.Value;
                }
                return string.Empty;
            }
        }

        //AdWords
        [DisplayName("AdWords Grouping")]
        public virtual string Grouping { get; set; }
        [DisplayName("AdWords Labels")]
        public virtual string Labels { get; set; }
        [DisplayName("AdWords Redirect")]
        public virtual string Redirect { get; set; }
    }
}