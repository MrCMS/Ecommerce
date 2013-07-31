using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class GoogleBaseSettings : SiteSettingsBase
    {
        public GoogleBaseSettings()
        {
            GoogleBaseTaxonomyFeedUrl = "http://www.google.com/basepages/producttype/taxonomy.en-GB.txt";
        }

        [Required]
        [DisplayName("Google Base Taxonomy Feed Url")]
        public string GoogleBaseTaxonomyFeedUrl { get; set; }

        [DisplayName("Default Category")]
        public string DefaultCategory{ get; set; }

        [DisplayName("Default Condition")]
        public ProductCondition DefaultCondition { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }
    }
}