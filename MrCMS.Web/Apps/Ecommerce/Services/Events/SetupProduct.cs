using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Events
{
    public class SetupProduct : IOnAdding<Product>
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;

        public SetupProduct(ISession session,IDocumentService documentService)
        {
            _session = session;
            _documentService = documentService;
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

            var mediaCategory = _documentService.GetDocumentByUrl<MediaCategory>("product-galleries");
            if (mediaCategory == null)
            {
                mediaCategory = new MediaCategory
                {
                    Name = "Product Galleries",
                    UrlSegment = "product-galleries",
                    IsGallery = true,
                    HideInAdminNav = true
                };
                _documentService.AddDocument(mediaCategory);
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

            _documentService.AddDocument(productGallery);
        }
    }
}