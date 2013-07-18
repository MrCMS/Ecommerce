using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
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
            AttributeOptions = new List<ProductAttributeOption>();
        }

        public virtual MediaCategory Gallery { get; set; }

        public virtual decimal Tax
        {
            get { return Variants.Sum(x => x.Tax); }
        }

        public virtual bool HasVariants
        {
            get { return Variants.Any(); }
        }

        public virtual IList<ProductVariant> Variants { get; set; }

        public virtual IList<ProductSpecificationValue> SpecificationValues { get; set; }

        public virtual string GetSpecification(string name)
        {
            var spec = SpecificationValues.FirstOrDefault(value => value.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name == name);
            return spec == null ? null : spec.Value;
        }

        public override void AdminViewData(ViewDataDictionary viewData, ISession session)
        {
            base.AdminViewData(viewData, session);
            viewData["brands"] = session.QueryOver<Brand>()
                                        .OrderBy(brand => brand.Name).Asc
                                        .Cacheable()
                                        .List()
                                        .BuildSelectItemList(brand => brand.Name, brand => brand.Id.ToString(),
                                                             brand => brand == Brand, "None selected");
        }

        public virtual IList<Category> Categories { get; set; }

        protected override void CustomInitialization(IDocumentService service, ISession session)
        {
            base.CustomInitialization(service, session);

            var productVariant = new ProductVariant
                                     {
                                         Name = this.Name,
                                         TrackingPolicy = TrackingPolicy.DontTrack,
                                         StockRemaining = 0,
                                         BasePrice = (decimal)0.00,
                                         Weight = 0
                                     };
            Variants.Add(productVariant);
            productVariant.Product = this;
            session.Transact(s => s.Save(productVariant));

            var mediaCategory = service.GetDocumentByUrl<MediaCategory>("product-galleries");
            if (mediaCategory == null)
            {
                mediaCategory = new MediaCategory
                                    {
                                        Name = "Product Galleries",
                                        UrlSegment = "product-galleries",
                                        IsGallery = true
                                    };
                service.AddDocument(mediaCategory);
            }
            var productGallery = new MediaCategory
                                     {
                                         Name = Name,
                                         UrlSegment = "product-galleries/" + UrlSegment,
                                         IsGallery = true,
                                         Parent = mediaCategory
                                     };
            Gallery = productGallery;

            service.AddDocument(productGallery);
        }

        public virtual string ContainerUrl
        {
            get
            {
                var webpage = (Parent as Webpage);
                return webpage != null ? webpage.LiveUrlSegment : string.Empty;
            }
        }

        public virtual IList<ProductAttributeOption> AttributeOptions { get; set; }

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

        public virtual string EditUrl
        {
            get { return "~/Admin/Webpage/Edit/" + Id; }
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
    }
}