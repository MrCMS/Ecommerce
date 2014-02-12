using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;
using NHibernate;

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

        public virtual string GetSpecification(string name)
        {
            var spec =
                SpecificationValues.FirstOrDefault(
                    value => value.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name == name);
            return spec == null ? null : spec.Value;
        }

        public virtual IList<Category> Categories { get; set; }

        protected override void CustomInitialization(IDocumentService service, ISession session)
        {
            base.CustomInitialization(service, session);

            if (!Variants.Any())
            {
                var productVariant = new ProductVariant
                    {
                        Name = Name,
                        TrackingPolicy = TrackingPolicy.DontTrack,
                    };
                Variants.Add(productVariant);
                productVariant.Product = this;
                session.Transact(s => s.Save(productVariant));
            }

            var mediaCategory = service.GetDocumentByUrl<MediaCategory>("product-galleries");
            if (mediaCategory == null)
            {
                mediaCategory = new MediaCategory
                    {
                        Name = "Product Galleries",
                        UrlSegment = "product-galleries",
                        IsGallery = true,
                        HideInAdminNav = true
                    };
                service.AddDocument(mediaCategory);
            }
            var productGallery = new MediaCategory
                {
                    Name = Name,
                    UrlSegment = "product-galleries/" + UrlSegment,
                    IsGallery = true,
                    Parent = mediaCategory,
                    HideInAdminNav = true
                };
            Gallery = productGallery;

            service.AddDocument(productGallery);
        }

        public virtual IList<ProductOption> Options { get; set; }

        public virtual Brand Brand { get; set; }

        [StringLength(500)]
        public virtual string Abstract { get; set; }

        public virtual IEnumerable<MediaFile> Images
        {
            get
            {
                return Gallery != null
                           ? (IEnumerable<MediaFile>)
                             Gallery.Files.Where(file => file.IsImage).OrderBy(file => file.DisplayOrder)
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
            get { return DisplayPreviousPrice.HasValue && DisplayPreviousPrice > DisplayPrice; }
        }

        public virtual IList<Product> RelatedProducts { get; set; }

        public virtual List<SelectListItem> GetVariantOptions(ProductVariant productVariant, bool showName = true, bool showOptionValues = true)
        {
            return Variants.BuildSelectItemList(variant => variant.GetSelectOptionName(showName, showOptionValues),
                                                       variant => variant.Id.ToString(),
                                                       variant => variant == productVariant,
                                                       emptyItem: null);
        }

        public override void OnDeleting(ISession session)
        {
            if (this.Categories.Count > 0)
            {
                foreach (var category in this.Categories)
                    category.Products.Remove(this);
                this.Categories.Clear();
            }
            base.OnDeleting(session);
        }
    }
}