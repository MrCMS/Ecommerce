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
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class CartController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetCart _getCart;
        private readonly ICartManager _cartManager;
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly ICountryService _countryService;
        private readonly IOrderService _orderService;

        public CartController(IGetCart getCart, ICartManager cartManager, IProductService productService,
            IProductVariantService productVariantService, ICountryService countryService, IOrderService orderService)
        {
            _getCart = getCart;
            _cartManager = cartManager;
            _productService = productService;
            _productVariantService = productVariantService;
            _countryService = countryService;
            _orderService = orderService;
        }

        public ViewResult Show(Cart page)
        {
            return View(page);
        }
        [HttpGet]
        public ViewResult CartPanel()
        {
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult AddToCart(int Id = 0, int IdVariant = 0, int quantity = 0)
        {
            Product product = _productService.Get(Id);
            ProductVariant productVariant = _productVariantService.Get(IdVariant);
            if (product != null && quantity > 0)
                _cartManager.AddToCart(product, quantity);
            if (productVariant != null && quantity > 0)
                _cartManager.AddToCart(productVariant, quantity);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
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
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ActionResult EnterOrderEmail()
        {
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            if (String.IsNullOrWhiteSpace(_getCart.GetOrderEmail()) && CurrentRequestData.CurrentUser != null)
                _getCart.SetOrderEmail(CurrentRequestData.CurrentUser.Email);
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult EnterOrderEmail(string email, bool havePassword=true)
        {
            if(!String.IsNullOrWhiteSpace(email))
            {
                _getCart.SetOrderEmail(email);
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            }
            else
                return Redirect(UniquePageHelper.GetUrl<EnterOrderEmail>());
        }
        [HttpGet]
        public ActionResult SetDeliveryDetails()
        {
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());

            if (_getCart.GetShippingAddress() == null)
            {
                if (CurrentRequestData.CurrentUser != null)
                {
                    Address address = new Address();
                    address.FirstName = CurrentRequestData.CurrentUser.FirstName != null ? CurrentRequestData.CurrentUser.FirstName : String.Empty;
                    address.LastName = CurrentRequestData.CurrentUser.LastName != null ? CurrentRequestData.CurrentUser.LastName : String.Empty;
                    address.UserGuid = CurrentRequestData.UserGuid;
                    _getCart.SetShippingAddress(address);
                }
                else
                {
                    Address address = new Address();
                    _getCart.SetShippingAddress(address);
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
            }
            return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
        }
        [HttpGet]
        public ActionResult PaymentDetails()
        {
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());

            List<SelectListItem> countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_getCart.GetBillingAddress()!= null && _getCart.GetBillingAddress().Country != null)
            {
                countries.Where(x => x.Value == _getCart.GetBillingAddress().Country.Id.ToString()).SingleOrDefault().Selected = true;
            }
            ViewData["countries"] = countries;

            if (_getCart.GetShippingAddress() == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            else
                return View(_getCart.GetCart());
        }
        [HttpPost]
        public ActionResult PaymentDetails(Address address, string CardNumber, string CardVerificationCode, 
            string NameOnCard, string CardIssueNumber, bool UseDeliveryAddress=false,
            int CartType=0, int StartMonth=0,int StartYear=0, int EndMonth=0, int EndYear=0)
        {
            if (address != null && !String.IsNullOrWhiteSpace(address.FirstName))
            {
                address.UserGuid = CurrentRequestData.UserGuid;
                _getCart.SetBillingAddress(address);
            }
            else
            {
                _getCart.SetBillingAddress(_getCart.GetShippingAddress());
            }

            if (_getCart.GetCart().Items.Count > 0)
            {
                //TODO: IMPLEMENT PAYMENT GATEWAY PROCEDURES

                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>()+"?orderID="+ _orderService.PlaceOrder(_getCart.GetCart()).ToString());
            }
            else
            {
                List<SelectListItem> countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
                if (_getCart.GetBillingAddress() != null && _getCart.GetBillingAddress().Country != null)
                {
                    countries.Where(x => x.Value == _getCart.GetBillingAddress().Country.Id.ToString()).SingleOrDefault().Selected = true;
                }
                ViewData["countries"] = countries;
                return View(_getCart.GetCart());
            }
        }
        
    }
}