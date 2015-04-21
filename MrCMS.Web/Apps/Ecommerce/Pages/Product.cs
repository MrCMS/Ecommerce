using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Product : Webpage
    {
        public Product()
        {
            Variants = new List<ProductVariant>();
            SpecificationValues = new List<ProductSpecificationValue>();
            Categories = new List<Category>();
            Options = new List<ProductOption>();
            RelatedProducts = new List<Product>();
        }

        public virtual MediaCategory Gallery { get; set; }

        public virtual bool IsMultiVariant
        {
            get { return Variants.Count > 1; }
        }

        public virtual IList<ProductVariant> Variants { get; set; }

        public virtual IList<ProductSpecificationValue> SpecificationValues { get; set; }

        public virtual IList<Category> Categories { get; set; }

        public virtual IList<ProductOption> Options { get; set; }

        public virtual Brand Brand { get; set; }

        [DisplayName("Abstract")]
        [StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string ProductAbstract { get; set; }

        public virtual IEnumerable<MediaFile> Images
        {
            get
            {
                return Gallery != null
                    ? (IEnumerable<MediaFile>)
                        Gallery.Files.Where(file => file.IsImage()).OrderBy(file => file.DisplayOrder)
                    : new List<MediaFile>();
            }
        }

        public virtual string DisplayImageUrl
        {
            get
            {
                return Images.Any()
                    ? Images.First().FileUrl
                    : MrCMSApplication.Get<EcommerceSettings>().DefaultNoProductImage;
            }
        }

        public virtual string EditUrl
        {
            get { return "/Admin/Webpage/Edit/" + Id; }
        }

        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal? DisplayPrice
        {
            get
            {
                if (Variants.Any())
                {
                    return Variants.Count == 1
                        ? Variants.First().Price
                        : Variants.OrderBy(x => x.Price).First().Price;
                }
                return null;
            }
        }

        [DisplayFormat(DataFormatString = "{0:£0.00}")]
        public virtual decimal? DisplayPreviousPrice
        {
            get
            {
                if (Variants.Any())
                {
                    return Variants.Count == 1
                        ? Variants.First().PreviousPriceIncludingTax
                        : Variants.OrderBy(x => x.Price).First().PreviousPriceIncludingTax;
                }
                return null;
            }
        }

        public virtual bool ShowPreviousPrice
        {
            get { return DisplayPreviousPrice != null && DisplayPreviousPrice > DisplayPrice; }
        }

        public virtual IList<Product> RelatedProducts { get; set; }


        public virtual IList<Product> PublishedRelatedProducts
        {
            get { return RelatedProducts.Where(x => x.Published).ToList(); }
        }

        public virtual string GetSpecification(string name)
        {
            ProductSpecificationValue spec =
                SpecificationValues.FirstOrDefault(
                    value => value.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name == name);
            return spec == null ? null : spec.Value;
        }

        public virtual List<SelectListItem> GetVariantOptions(ProductVariant productVariant, bool showName = true,
            bool showOptionValues = true)
        {
            return Variants.BuildSelectItemList(variant => variant.GetSelectOptionName(showName, showOptionValues),
                variant => variant.Id.ToString(),
                variant => variant == productVariant,
                emptyItem: null);
        }
    }
}