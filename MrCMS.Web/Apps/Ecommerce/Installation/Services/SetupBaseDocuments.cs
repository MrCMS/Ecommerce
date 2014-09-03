using System;
using System.Linq;
using MrCMS.Entities.Documents.Web.FormProperties;
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
        private readonly IFormAdminService _formAdminService;

        public SetupBaseDocuments(IDocumentService documentService, IPageTemplateAdminService pageTemplateAdminService, IFormAdminService formAdminService)
        {
            _documentService = documentService;
            _pageTemplateAdminService = pageTemplateAdminService;
            _formAdminService = formAdminService;
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
                RevealInNavigation = false
            };
            _documentService.AddDocument(productSearch);
            _documentService.PublishNow(productSearch);
            _documentService.AddDocument(categoryContainer);
            _documentService.PublishNow(categoryContainer);
            pageModel.ProductSearch = productSearch;

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
            var addedToCart = new ProductAddedToCart
            {
                Name = "Added to Basket",
                UrlSegment = "add-to-basket",
                RevealInNavigation = false,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(addedToCart);
            pageModel.ProductAddedToCart = addedToCart;

            var wishlist = new ShowWishlist
            {
                Name = "Wishlist",
                UrlSegment = "wishlist",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(wishlist);

            var newIn = new NewInProducts
            {
                Name = "New In",
                UrlSegment = "new-in",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow
            };
            _documentService.AddDocument(newIn);

            var about = new TextPage()
            {
                Name = "About us",
                UrlSegment = "about-us",
                RevealInNavigation = true,
                PublishOn = DateTime.UtcNow,
                BodyContent = EcommerceInstalInfo.AboutUsText
            };
            _documentService.AddDocument(about);

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

                homePage.SubmitButtonText = "Sign up";
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
                Latitude = 55.01021m,
                Longitude = -1.44998m,
                Address = EcommerceInstalInfo.Address,
                PinImage = mediaModel.Logo.FileUrl,
                BodyContent = "[form]",
                FormDesign = EcommerceInstalInfo.ContactFormDesign
            };
            _documentService.AddDocument(contactUs);
            GetFormProperties(contactUs);

            return pageModel;
        }

        private void GetFormProperties(ContactUs contactus)
        {
            contactus = _documentService.GetDocument<ContactUs>(contactus.Id);
            var name = new TextBox
            {
                Name = "Name",
                Required = true,
                Webpage = contactus,
                DisplayOrder = 1,
                CssClass = "input-lg form-control"
            };
            _formAdminService.AddFormProperty(name);
            var email = new TextBox
            {
                Name = "Email",
                Required = true,
                Webpage = contactus,
                DisplayOrder = 2,
                CssClass = "input-lg form-control"
            };
            _formAdminService.AddFormProperty(email);
            var contact = new TextBox
            {
                Name = "Telephone Number",
                Required = true,
                Webpage = contactus,
                DisplayOrder = 3,
                CssClass = "input-lg form-control"
            };
            _formAdminService.AddFormProperty(contact);
            var message = new TextArea
            {
                Name = "Message",
                Required = true,
                Webpage = contactus,
                DisplayOrder = 4,
                CssClass = "input-lg form-control"
            };
            _formAdminService.AddFormProperty(message);
        }
    }
}