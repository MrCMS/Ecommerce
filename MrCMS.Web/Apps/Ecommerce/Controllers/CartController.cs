using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Cart;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using System;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using System.Linq;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Shipping;
using MrCMS.Web.Apps.Ecommerce.Services.Shipping;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Services.Discounts;

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
        private readonly IShippingMethodManager _shippingMethodManager;
        private readonly IDiscountManager _discountManager;

        public CartController(IGetCart getCart, ICartManager cartManager, IProductService productService,
            IProductVariantService productVariantService, ICountryService countryService, IOrderService orderService,
            IShippingMethodManager shippingMethodManager, IDiscountManager discountManager)
        {
            _getCart = getCart;
            _cartManager = cartManager;
            _productService = productService;
            _productVariantService = productVariantService;
            _countryService = countryService;
            _orderService = orderService;
            _shippingMethodManager = shippingMethodManager;
            _discountManager = discountManager;
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
        public RedirectResult AddToCart(ProductVariant productVariant, int quantity = 0)
        {
            if (productVariant != null && quantity > 0)
                _cartManager.AddToCart(productVariant, quantity);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
        }
        [HttpGet]
        public ViewResult EditCartItem(CartItem item)
        {
            return View("EditCartItem",item);
        }
        [ActionName("EditCartItem")]
        [HttpPost]
        public ActionResult EditCartItem_POST(CartItem item)
        {
            if (ModelState.IsValid)
            {
                _cartManager.UpdateQuantity(item, item.Quantity);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(item);
        }
        [HttpGet]
        public PartialViewResult DeleteCartItem(CartItem item)
        {
            return PartialView(item);
        }
        [ActionName("DeleteCartItem")]
        [HttpPost]
        public ActionResult DeleteCartItem_POST(CartItem item)
        {
            _cartManager.Delete(item);
            return Redirect(UniquePageHelper.GetUrl<Cart>());
        }
        [HttpGet]
        public ViewResult DiscountCode()
        {
            return View(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult AddDiscountCode()
        {
            return View(_getCart.GetCart());
        }
        [ActionName("AddDiscountCode")]
        [HttpPost]
        public ActionResult AddDiscountCode_POST(CartModel model)
        {
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) && ValidateDiscountCode(model.DiscountCode))
            {
                _getCart.SetDiscountCode(model.DiscountCode);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(_getCart.GetCart());
        }
        [HttpGet]
        public ViewResult EditDiscountCode()
        {
            return View(_getCart.GetCart());
        }
        [ActionName("EditDiscountCode")]
        [HttpPost]
        public ActionResult EditDiscountCode_POST(CartModel model)
        {
            if (model != null && !String.IsNullOrWhiteSpace(model.DiscountCode) && ValidateDiscountCode(model.DiscountCode))
            {
                _getCart.SetDiscountCode(model.DiscountCode);
                return Redirect(UniquePageHelper.GetUrl<Cart>());
            }
            return PartialView(_getCart.GetCart());
        }
        public JsonResult IsDiscountCodeValid(string discountCode)
        {
            return Json(ValidateDiscountCode(discountCode), JsonRequestBehavior.AllowGet);
        }

        private bool ValidateDiscountCode(string discountCode)
        {
            if (!String.IsNullOrWhiteSpace(discountCode))
            {
                var discount = _discountManager.GetByCode(discountCode);
                if (discount != null)
                {
                    if (discount.IsCodeValid(discountCode))
                        return true;
                }
            }
            
            return false;
        }
        [HttpGet]
        public ViewResult Details()
        {
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public JsonResult UpdateQuantity(int quantity=0,int cartId=0)
        {
            if (quantity > 0 && cartId != 0)
            {
                var cartItem=_getCart.GetCart().Items.SingleOrDefault(x => x.Id==cartId);
                if(cartItem!=null)
                    _cartManager.UpdateQuantity(cartItem, quantity);
                return Json(true);
            }
            return Json(false);
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
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult EnterOrderEmail(string OrderEmail, bool havePassword=true)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrWhiteSpace(OrderEmail))
                {
                    _getCart.SetOrderEmail(OrderEmail);
                    return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
                }
            }
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
                    var address = new Address
                        {
                            FirstName = CurrentRequestData.CurrentUser.FirstName ?? String.Empty,
                            LastName = CurrentRequestData.CurrentUser.LastName ?? String.Empty,
                            UserGuid = CurrentRequestData.UserGuid
                        };
                    _getCart.SetShippingAddress(address);
                }
                else
                {
                    var address = new Address();
                    _getCart.SetShippingAddress(address);
                }
            }
            var countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_getCart.GetShippingAddress().Country != null)
            {
                countries.SingleOrDefault(x => x.Value == _getCart.GetShippingAddress().Country.Id.ToString()).Selected = true;
            }
            ViewData["countries"] = countries;
            return View(_getCart.GetCart());
        }
        [HttpPost]
        public RedirectResult SetDeliveryDetails(Address address, ShippingMethod shippingMethod)
        {
            if (address != null && shippingMethod!=null)
            {
                address.UserGuid = CurrentRequestData.UserGuid;
                _getCart.SetShippingAddress(address);
                _getCart.SetShippingMethod(shippingMethod.Id);
                return Redirect(UniquePageHelper.GetUrl<PaymentDetails>());
            }
            return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
        }
        [HttpGet]
        public ActionResult PaymentDetails()
        {
            if (_getCart.GetCart().Items.Count == 0)
                return Redirect(UniquePageHelper.GetUrl<Cart>());

            var countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
            if (_getCart.GetBillingAddress()!= null && _getCart.GetBillingAddress().Country != null)
            {
                countries.SingleOrDefault(x => x.Value == _getCart.GetBillingAddress().Country.Id.ToString()).Selected = true;
            }
            ViewData["countries"] = countries;

            if (_getCart.GetShippingAddress() == null)
                return Redirect(UniquePageHelper.GetUrl<SetDeliveryDetails>());
            else
                return View(new PaymentDetailsModel(){CartModel = _getCart.GetCart()});
        }
        [HttpPost]
        public ActionResult PaymentDetails(Address address, PaymentDetailsModel paymentDetailsModel)
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
            if (paymentDetailsModel != null && !String.IsNullOrWhiteSpace(paymentDetailsModel.CartType))
            {
                _getCart.SetPaymentMethod(paymentDetailsModel.CartType);
            }
            
            if (_getCart.GetCart().Items.Count > 0)
            {
                //TODO: IMPLEMENT PAYMENT GATEWAY PROCEDURES

                return Redirect(UniquePageHelper.GetUrl<OrderPlaced>()+"?orderID="+ _orderService.PlaceOrder(_getCart.GetCart()).ToString());
            }
            else
            {
                var countries = _countryService.GetAllCountries().BuildSelectItemList(country => country.Name, country => country.Id.ToString(), null, emptyItem: null);
                if (_getCart.GetBillingAddress() != null && _getCart.GetBillingAddress().Country != null)
                {
                    countries.SingleOrDefault(x => x.Value == _getCart.GetBillingAddress().Country.Id.ToString()).Selected = true;
                }
                ViewData["countries"] = countries;
                return View(_getCart.GetCart());
            }
        }
        [HttpGet]
        public ActionResult ShippingMethods(int id=0)
        {
            if (id != 0)
            {
                var country = _countryService.Get(id);
                if (country != null)
                {
                    return View(country.GetShippingMethods().Where(x => x.CanBeUsed(_getCart.GetCart())).ToList());
                }
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddShippingMethod(int id = 0)
        {
            if (id != 0)
            {
                var shippingMethod = _shippingMethodManager.Get(id);
                if (shippingMethod != null)
                {
                    _getCart.SetShippingMethod(shippingMethod.Id);
                    return Json(true);
                }
            }
            else
            {
                _getCart.SetShippingMethod(0);
            }
            return Json(false);
        }
    }
}