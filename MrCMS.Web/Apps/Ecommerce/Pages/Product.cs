using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Product : Webpage, ICanAddToCart
    {
        public Product()
        {
            Variants = new List<ProductVariant>();
            SpecificationValues = new List<ProductSpecificationValue>();
            Categories = new List<Category>();
        }

        public virtual MediaCategory Gallery { get; set; }

        public virtual ProductAvailability Availability
        {
            get
            {
                if (PublishOn.HasValue && PublishOn <= DateTime.UtcNow)
                    return ProductAvailability.Available;
                return ProductAvailability.PreOrder;
            }
        }

        public virtual bool InStock
        {
            get { return !StockRemaining.HasValue || StockRemaining > 0; }
        }

        public virtual string SKU { get; set; }

        public virtual decimal TaxRatePercentage
        {
            get
            {
                return TaxRate == null
                           ? 0
                           : TaxRate.Percentage;
            }
        }

        [DisplayName("Stock Remaining")]
        public virtual int? StockRemaining { get; set; }

        [DisplayName("Price Pre Tax")]
        public virtual decimal PricePreTax { get; set; }

        public virtual decimal ReducedBy
        {
            get
            {
                return !PreviousPrice.HasValue
                           ? 0
                           : PreviousPrice > Price
                                 ? PreviousPrice.Value - Price
                                 : 0;
            }
        }

        [DisplayName("Previous Price")]
        public virtual decimal? PreviousPrice { get; set; }

        public virtual decimal ReducedByPercentage
        {
            get { return PreviousPrice.HasValue ? ReducedBy / PreviousPrice.Value : 0; }
        }

        public virtual decimal Price
        {
            get
            {
                return Math.Round(TaxRate != null
                                      ? PricePreTax * (TaxRate.Multiplier)
                                      : PricePreTax, 2, MidpointRounding.AwayFromZero);
            }
        }

        public virtual TaxRate TaxRate { get; set; }

        public virtual decimal Tax
        {
            get { return Price - PricePreTax; }
        }

        public virtual bool HasVariants
        {
            get { return Variants.Any(); }
        }

        public virtual IList<ProductVariant> Variants { get; set; }
        public virtual IList<ProductSpecificationValue> SpecificationValues { get; set; }

        public virtual bool CanBuy(int quantity)
        {
            return quantity > 0 && (!StockRemaining.HasValue || StockRemaining >= quantity);
        }

        public virtual string GetSpecification(string name)
        {
            var spec = SpecificationValues.FirstOrDefault(value => value.Option.Name == name);
            if (spec == null)
                return null;
            return spec.Value;
        }

        public override void AdminViewData(ViewDataDictionary viewData, ISession session)
        {
            base.AdminViewData(viewData, session);
            viewData["taxrates"] = session.QueryOver<TaxRate>()
                                          .Cacheable()
                                          .List()
                                          .BuildSelectItemList(rate => rate.Name, rate => rate.Id.ToString(),
                                                               rate => rate == TaxRate, "None selected");

        }

        public virtual IList<Category> Categories { get; set; }

        protected override void CustomInitialization(IDocumentService service, ISession session)
        {
            base.CustomInitialization(service, session);

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
    }
}