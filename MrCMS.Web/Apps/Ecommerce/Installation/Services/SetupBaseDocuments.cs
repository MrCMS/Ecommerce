using System;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupBaseDocuments : ISetupBaseDocuments
    {
        private readonly IWebpageAdminService _webpageAdminService;
        private readonly IGetDocumentByUrl<TextPage> _getByUrl;
        private readonly ISession _session;
        private readonly IPageTemplateAdminService _pageTemplateAdminService;
        private readonly IFormAdminService _formAdminService;

        public SetupBaseDocuments(IWebpageAdminService webpageAdminService, IPageTemplateAdminService pageTemplateAdminService, IFormAdminService formAdminService, IGetDocumentByUrl<TextPage> getByUrl, ISession session)
        {
            _webpageAdminService = webpageAdminService;
            _pageTemplateAdminService = pageTemplateAdminService;
            _formAdminService = formAdminService;
            _getByUrl = getByUrl;
            _session = session;
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
            _webpageAdminService.Add(productSearch);
            _webpageAdminService.PublishNow(productSearch);
            _webpageAdminService.Add(categoryContainer);
            _webpageAdminService.PublishNow(categoryContainer);
            pageModel.ProductSearch = productSearch;

            var now = DateTime.UtcNow;
            var yourBasket = new Cart
            {
                Name = "Your Basket",
                UrlSegment = "basket",
                RevealInNavigation = false,
                PublishOn = now
            };
            _webpageAdminService.Add(yourBasket);
            var enterOrderEmail = new EnterOrderEmail
            {
                Name = "Enter Order Email",
                UrlSegment = "enter-order-email",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 0,
                PublishOn = now,
            };
            _webpageAdminService.Add(enterOrderEmail);
            var setPaymentDetails = new PaymentDetails
            {
                Name = "Set Payment Details",
                UrlSegment = "set-payment-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 1,
                PublishOn = now,
            };
            _webpageAdminService.Add(setPaymentDetails);
            var setDeliveryDetails = new SetShippingDetails
            {
                Name = "Set Shipping Details",
                UrlSegment = "set-shipping-details",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 2,
                PublishOn = now,
            };
            _webpageAdminService.Add(setDeliveryDetails);
            var orderPlaced = new OrderPlaced
            {
                Name = "Order Placed",
                UrlSegment = "order-placed",
                RevealInNavigation = false,
                Parent = yourBasket,
                DisplayOrder = 3,
                PublishOn = now,
            };
            _webpageAdminService.Add(orderPlaced);

            // User Account
            var userAccount = new SitemapPlaceholder
            {
                Name = "User Account",
                UrlSegment = "user-account",
                RevealInNavigation = false,
                PublishOn = now
            };
            _webpageAdminService.Add(userAccount);

            var userAccountInfo = new UserAccountInfo
            {
                Name = "Account Details",
                UrlSegment = "user-account-details",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountInfo);

            var userAccountPassword = new UserAccountChangePassword
            {
                Name = "Change Password",
                UrlSegment = "user-account-change-password",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountPassword);

            var userAccountAddresses = new UserAccountAddresses
            {
                Name = "Account Addresses",
                UrlSegment = "user-account-addresses",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountAddresses);

            var editAddress = new UserAccountEditAddress
            {
                Name = "Edit Address",
                UrlSegment = userAccountAddresses.UrlSegment + "/edit-address",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccountAddresses
            };
            _webpageAdminService.Add(editAddress);

            var userAccountOrders = new UserAccountOrders
            {
                Name = "Orders",
                UrlSegment = "user-account-orders",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountOrders);

            var userOrder = new UserOrder
            {
                Name = "View Order",
                UrlSegment = "user-account-orders/order",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccountOrders
            };
            _webpageAdminService.Add(userOrder);

            var userAccountReviews = new UserAccountReviews
            {
                Name = "Reviews",
                UrlSegment = "user-account-reviews",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountReviews);

            var userAccountRewards = new UserAccountRewardPoints
            {
                Name = "Reward Points",
                UrlSegment = "user-account-reward-points",
                RevealInNavigation = false,
                PublishOn = now,
                Parent = userAccount
            };
            _webpageAdminService.Add(userAccountRewards);

            // End User Account


            //Added to cart
            var addedToCart = new ProductAddedToCart
            {
                Name = "Added to Basket",
                UrlSegment = "add-to-basket",
                RevealInNavigation = false,
                PublishOn = now
            };
            _webpageAdminService.Add(addedToCart);
            pageModel.ProductAddedToCart = addedToCart;

            var wishlist = new ShowWishlist
            {
                Name = "Wishlist",
                UrlSegment = "wishlist",
                RevealInNavigation = true,
                PublishOn = now
            };
            _webpageAdminService.Add(wishlist);

            var newIn = new NewInProducts
            {
                Name = "New In",
                UrlSegment = "new-in",
                RevealInNavigation = true,
                PublishOn = now
            };
            _webpageAdminService.Add(newIn);

            var about = new TextPage()
            {
                Name = "About us",
                UrlSegment = "about-us",
                RevealInNavigation = true,
                PublishOn = now,
                BodyContent = EcommerceInstallInfo.AboutUsText
            };
            _webpageAdminService.Add(about);

            //update core pages
            var homePage = _getByUrl.GetByUrl("home");
            if (homePage != null)
            {
                homePage.BodyContent = EcommerceInstallInfo.HomeCopy;
                var templates = _pageTemplateAdminService.Search(new PageTemplateSearchQuery());
                var homeTemplate = templates.FirstOrDefault(x => x.Name == "Home Page");
                if (homeTemplate != null)
                {
                    homePage.PageTemplate = homeTemplate;
                }

                homePage.SubmitButtonText = "Sign up";
                _webpageAdminService.Update(homePage);
                pageModel.HomePage = homePage;
            }
            var page2 = _getByUrl.GetByUrl("page-2");
            if (page2 != null)//demopage in core not needed
                _webpageAdminService.Delete(page2);

            var contactus = _getByUrl.GetByUrl("contact-us");
            if (contactus != null)//demopage in core not needed
                _webpageAdminService.Delete(contactus);

            //Added to cart
            var contactUs = new ContactUs()
            {
                Name = "Contact Us",
                UrlSegment = "contact-us",
                RevealInNavigation = true,
                PublishOn = now,
                Latitude = 55.01021m,
                Longitude = -1.44998m,
                Address = EcommerceInstallInfo.Address,
                PinImage = mediaModel.Logo.FileUrl,
                BodyContent = "[form]",
                FormDesign = EcommerceInstallInfo.ContactFormDesign
            };
            _webpageAdminService.Add(contactUs);
            GetFormProperties(contactUs);

            var brandListing = new BrandListing
            {
                Name = "Brands",
                UrlSegment = "brands",
                RevealInNavigation = true,
                PublishOn = now,
                BodyContent = ""
            };
            _webpageAdminService.Add(brandListing);

            return pageModel;
        }

        private void GetFormProperties(ContactUs contactus)
        {
            contactus = _session.Get<ContactUs>(contactus.Id);
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