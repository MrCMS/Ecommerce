using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Events
{
    public class SetupProduct : IOnAdding<Product>
    {
        private readonly ISession _session;
        private readonly IGetDocumentByUrl<MediaCategory> _mediaCategoryByUrl;
        private readonly IMediaCategoryAdminService _mediaCategoryAdminService;

        public SetupProduct(ISession session, IGetDocumentByUrl<MediaCategory> mediaCategoryByUrl,IMediaCategoryAdminService mediaCategoryAdminService)
        {
            _session = session;
            _mediaCategoryByUrl = mediaCategoryByUrl;
            _mediaCategoryAdminService = mediaCategoryAdminService;
        }

        public void Execute(OnAddingArgs<Product> args)
        {
            var product = args.Item;
            if (!product.Variants.Any())
            {
                var productVariant = new ProductVariant
                {
                    Name = product.Name,
                    TrackingPolicy = TrackingPolicy.DontTrack,
                };
                product.Variants.Add(productVariant);
                productVariant.Product = product;
                _session.Transact(s => s.Save(productVariant));
            }

            var mediaCategory = _mediaCategoryByUrl.GetByUrl("product-galleries");
            if (mediaCategory == null)
            {
                mediaCategory = new MediaCategory
                {
                    Name = "Product Galleries",
                    UrlSegment = "product-galleries",
                    IsGallery = true,
                    HideInAdminNav = true
                };
                _mediaCategoryAdminService.Add(mediaCategory);
            }
            var productGallery = new MediaCategory
            {
                Name = product.Name,
                UrlSegment = "product-galleries/" + product.UrlSegment,
                IsGallery = true,
                Parent = mediaCategory,
                HideInAdminNav = true
            };
            product.Gallery = productGallery;

            _mediaCategoryAdminService.Add(productGallery);
        }
    }
}