using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using System.Collections.Generic;
using System.Linq;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;
        private readonly ICartManager _cartManager;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly ICountryService _countryService;

        public CartController(IGetCart getCart, ICartManager cartManager, IProductService productService, IProductVariantService productVariantService, ICountryService countryService)
        {
            _getCart = getCart;
            _cartManager = cartManager;
            _productService = productService;
            _productVariantService = productVariantService;
            _countryService = countryService;
        }

        public ViewResult Show(Cart page)
        {
            return View(page);
        }

        [HttpGet]
        public ViewResult Details()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult BasicDetails()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult DeliveryDetails()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult OrderEmail()
        {
            ViewBag.EnterOrderEmailUrl = UniquePageHelper.GetUrl<EnterOrderEmail>();
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult EnterOrderEmail()
        {
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult EnterOrderEmail(string email, bool havePassword=true)
        {
            _getCart.SetOrderEmail(email);
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }
        [HttpGet]
        public ActionResult OrderPlaced()
        {
            if (_getCart.GetShippingAddress() == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            else
                return View(_getCart.GetCart());
        }
        [HttpGet]
        public ActionResult PaymentDetails()
        {
            if (_getCart.GetShippingAddress() == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            else
            {

                return View(_getCart.GetCart());
            }
        }
        [HttpGet]
        public ViewResult SetDeliveryDetails()
        {
            if (_getCart.GetShippingAddress() == null)
            {
                if (CurrentRequestData.CurrentUser != null)
                {
                    Address address = new Address();
                    address.FirstName = CurrentRequestData.CurrentUser.FirstName != null ? CurrentRequestData.CurrentUser.FirstName : String.Empty;
                    address.LastName = CurrentRequestData.CurrentUser.LastName != null ? CurrentRequestData.CurrentUser.LastName : String.Empty;
                    address.UserGuid = CurrentRequestData.UserGuid;
                    _getCart.SetShippingAddress(address);
                    _getCart.SetBillingAddress(address);
                }
                else
                {
                    Address address = new Address();
                    _getCart.SetShippingAddress(address);
                    _getCart.SetBillingAddress(address);
                }
            }
            List<SelectListItem> countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_getCart.GetShippingAddress().Country != null)
            {
                countries.Where(x => x.Value == _getCart.GetShippingAddress().Country.Id.ToString()).SingleOrDefault().Selected = true;
            }
            ViewData["countries"] = countries;
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult SetDeliveryDetails(Address address)
        {
            if (address != null)
            {
                address.UserGuid = CurrentRequestData.UserGuid;
                _getCart.SetShippingAddress(address);
                _getCart.SetBillingAddress(address);
            }
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }
        [HttpGet]
        public ViewResult CartPanel()
        {
            ViewBag.BasketUrl = UniquePageHelper.GetUrl<EnterOrderEmail>();
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult AddToCart(int Id=0, int IdVariant=0,int quantity=0)
        {
            Product product = _productService.Get(Id);
            ProductVariant productVariant = _productVariantService.Get(IdVariant);
            if (product != null && quantity>0)
                _cartManager.AddToCart(product, quantity);
            if (productVariant != null && quantity > 0)
                _cartManager.AddToCart(productVariant, quantity);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
        }
    }
}