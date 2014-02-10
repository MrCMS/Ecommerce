using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using System.Collections.Generic;
using System;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Settings;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class NewInProductsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IProductService _productService;
        private readonly CartModel _cartModel;

        public NewInProductsController(IProductService productService, CartModel cartModel)
        {
            _productService = productService;
            _cartModel = cartModel;
        }

        public ViewResult Show(NewInProducts page)
        {
            ViewData["newinproducts"] = _productService.GetNewIn(12);
            ViewData["cart"] = _cartModel;
            return View(page);
        }
    }
}