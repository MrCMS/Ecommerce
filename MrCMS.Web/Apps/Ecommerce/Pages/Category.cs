using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Category : EcommerceSearchablePage
    {
        private string _nestedName;

        public Category()
        {
            Products = new List<Product>();
        }

        public virtual string NestedName
        {
            get { return _nestedName ?? (_nestedName = GetNestedName()); }
        }

        public virtual IList<Product> Products { get; set; }

        public virtual string ContainerUrl
        {
            get { return (Parent as Webpage).LiveUrlSegment; }
        }

        [DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }

        public virtual string DisplayImageUrl
        {
            get
            {
                return (string.IsNullOrEmpty(FeatureImage)
                    ? MrCMSApplication.Get<EcommerceSettings>().DefaultNoProductImage
                    : FeatureImage);
            }
        }

        [DisplayName("Show sub categories")]
        public virtual bool ShowSubCategories { get; set; }

        [StringLength(500), DisplayName("Abstract")]
        public virtual string CategoryAbstract { get; set; }

        [DisplayName("Default Product Search Sort")]
        public virtual ProductSearchSort? DefaultProductSearchSort { get; set; }

        private string GetNestedName()
        {
            IEnumerable<Webpage> categories = ActivePages.TakeWhile(webpage => webpage.Unproxy() is Category).Reverse();

            return string.Join(" > ", categories.Select(webpage => webpage.Name));
        }
    }
}