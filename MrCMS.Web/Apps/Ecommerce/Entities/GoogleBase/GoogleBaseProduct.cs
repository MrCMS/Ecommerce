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
        }
        [DisplayName("Product Condition")]
        public virtual ProductCondition? OverrideCondition { get; set; }
        public virtual ProductCondition Condition
        {
            get
            {
                return OverrideCondition.HasValue ? OverrideCondition.Value : MrCMSApplication.Get<GoogleBaseSettings>().DefaultCondition;
            }
        }

        //Clothing & Accessories
        public virtual string Gender { get; set; }
        [DisplayName("Age Group")]
        public virtual string AgeGroup { get; set; }

        //Attributes
        public virtual string Material
        {
            get
            {
                if (ProductVariant.AttributeValues != null && ProductVariant.AttributeValues.Any())
                {
                    var optionValue = ProductVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Name == "Material");
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
                if (ProductVariant.AttributeValues != null && ProductVariant.AttributeValues.Any())
                {
                    var optionValue = ProductVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Name == "Pattern");
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
                if (ProductVariant.AttributeValues != null && ProductVariant.AttributeValues.Any())
                {
                    var optionValue = ProductVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Name == "Color" || x.ProductAttributeOption.Name == "Colour");
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
                if (ProductVariant.AttributeValues != null && ProductVariant.AttributeValues.Any())
                {
                    var optionValue = ProductVariant.AttributeValues.SingleOrDefault(x => x.ProductAttributeOption.Name == "Size");
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