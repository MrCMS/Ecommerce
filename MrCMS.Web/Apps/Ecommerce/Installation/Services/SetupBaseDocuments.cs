using System;
using System.Linq;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupBaseDocuments : ISetupBaseDocuments
    {
        private readonly IDocumentService _documentService;
        private readonly IPageTemplateAdminService _pageTemplateAdminService;

        public SetupBaseDocuments(IDocumentService documentService, IPageTemplateAdminService pageTemplateAdminService)
        {
            _documentService = documentService;
            _pageTemplateAdminService = pageTemplateAdminService;
        }

        public PageModel Setup(MediaModel mediaModel)
        {
            var pageModel = new PageModel();

            var productSearch = new ProductSearch
            {
                Name = "Categories",
                UrlSegment = "categories",
                RevealInNavigation = true
            };
            var categoryContainer = new ProductContainer
            {
                Name = "Products",
                UrlSegment = "products",
                RevealInNavigation = true
            };
            _documentService.AddDocument(productSearch);
            _documentService.PublishNow(productSearch);
            _documentService.AddDocument(categoryContainer);
            _documentService.PublishNow(categoryContainer);

            var welcome = new TextPage
            {
                Name = "Welcome",
                UrlSegment = "shop",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(welcome);
            var yourBasket = new Cart
            {
                Name = "Your Basket",
                UrlSegment = "basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(yourBasket);
            var enterOrderEmail = new EnterOrderEmail
            {
                Name = "Enter Order Email",
                UrlSegment = "enter-order-email",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 0,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(enterOrderEmail);
            var setPaymentDetails = new PaymentDetails
            {
                Name = "Set Payment Details",
                UrlSegment = "set-payment-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 1,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(setPaymentDetails);
            var setDeliveryDetails = new SetShippingDetails
            {
                Name = "Set Delivery Details",
                UrlSegment = "set-delivery-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 2,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(setDeliveryDetails);
            var orderPlaced = new OrderPlaced
            {
                Name = "Order Placed",
                UrlSegment = "order-placed",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = DateTime.UtcNow,
            };
            _documentService.AddDocument(orderPlaced);

            //Added to cart
            var addedToCart = new ProductAddedToCart()
            {
                Name = "Added to Basket",
                UrlSegment = "add-to-basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(addedToCart);


            //update core pages
            var homePage = _documentService.GetDocumentByUrl<TextPage>("home");
            if (homePage != null)
            {
                homePage.BodyContent = EcommerceInstalInfo.HomeCopy;
                var templates = _pageTemplateAdminService.Search(new PageTemplateSearchQuery());
                var homeTemplate = templates.FirstOrDefault(x => x.Name == "Home Page");
                if (homeTemplate != null)
                {
                    homePage.PageTemplate = homeTemplate;
                }
                _documentService.SaveDocument(homePage);
                pageModel.HomePage = homePage;
            }
            var page2 = _documentService.GetDocumentByUrl<TextPage>("page-2");
            if (page2 != null)//demopage in core not needed
                _documentService.DeleteDocument(page2);

            var contactus = _documentService.GetDocumentByUrl<TextPage>("contact-us");
            if (contactus != null)//demopage in core not needed
                _documentService.DeleteDocument(contactus);

            //Added to cart
            var contactUs = new ContactUs()
            {
                Name = "Contact Us",
                UrlSegment = "contact-us",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow,
                Longitude = 55.01021m,
                Latitude = -1.44998m,
                Address = EcommerceInstalInfo.Address,
                PinImage = mediaModel.Logo.FileUrl
            };
            _documentService.AddDocument(contactUs);

            return pageModel;
        }
    }
}