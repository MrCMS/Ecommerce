using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.EcommerceApp.Tests.Pages.ProductTests
{
    public class ProductPersistenceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void Product_Save_CanSaveAProduct()
        {
            var product = new Product();

            Session.Transact(session => session.Save(product));
        }

        [Fact]
        public void Product_Save_CanSaveAProductViaDocumentService()
        {
            var product = new Product();
            var product2 = new Product();
            var documentService = new DocumentService(Session, new SiteSettings(), new CurrentSite(CurrentSite));

            documentService.SaveDocument(product);
            documentService.SaveDocument(product2);
        }

        [Fact]
        public void Product_Save_CanAddAProductViaDocumentService()
        {
            var product = new Product();
            var product2 = new Product();
            var documentService = new DocumentService(Session, new SiteSettings(), new CurrentSite(CurrentSite));

            documentService.AddDocument(product);
            documentService.AddDocument(product2);
        }

        [Fact]
        public void Product_Save_CanSaveAProductAttachedToProductSearchViaDocumentService()
        {
            var productSearch = new ProductSearch();
            Session.Transact(session => session.Save(productSearch));
            var product = new Product { Parent = productSearch };
            var documentService = new DocumentService(Session, new SiteSettings(), new CurrentSite(CurrentSite));

            documentService.SaveDocument(product);
        }

        [Fact]
        public void Product_Save_CanAddAProductAttachedToProductSearchViaDocumentService()
        {
            var productSearch = new ProductSearch();
            Session.Transact(session => session.Save(productSearch));
            var product = new Product { Parent = productSearch };
            var product2 = new Product { Parent = productSearch };
            var documentService = new DocumentService(Session, new SiteSettings(), new CurrentSite(CurrentSite));

            documentService.AddDocument(product);
            documentService.AddDocument(product2);
        }
    }
}