using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;

using System;
using System.Collections.Generic;
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductService _productService;
        private readonly IProductSearchService _productSearchService;

        public ProductSearchController(ICategoryService categoryService, IProductOptionManager productOptionManager, IProductService productService, IProductSearchService productSearchService)
        {
            _categoryService = categoryService;
            _productOptionManager = productOptionManager;
            _productService = productService;
            _productSearchService = productSearchService;
        }

        public ViewResult Show(ProductSearch page)
        {
            ProductPagedList products = _productService.Search();
            ViewBag.ProductOptions = _productOptionManager.GetAllAttributeOptions();
            ViewBag.ProductSpecifications = _productOptionManager.ListSpecificationAttributes();
            decimal productPriceRangeMin = 0;
            decimal productPriceRangeMax = 0;
            foreach (var product in products)
            {
                if (product.Variants.Any())
                {
                    foreach (var variant in product.Variants)
                    {
                        if (variant.Price < productPriceRangeMin)
                            productPriceRangeMin = variant.Price;
                        if (variant.Price > productPriceRangeMax)
                            productPriceRangeMax = variant.Price;
                    }
                }
            }
            ViewBag.ProductPriceRangeMin = productPriceRangeMin;
            ViewBag.ProductPriceRangeMax = productPriceRangeMax;
            ViewBag.Categories = _categoryService.GetAll();
            ViewBag.Products = products;
            return View(page);
        }

        [HttpGet]
        public PartialViewResult Results(string options,string specifications, int pageNo = 0, decimal productPriceRangeMin = 0, decimal productPriceRangeMax = 0)
        {
            List<string> specifications2 = new List<string>();
            //specifications2.Add("width:1");
            List<string> options2 = new List<string>();
            //options2.Add("storage16,networklte,");
            ProductPagedList products = new ProductPagedList(_productSearchService.SearchProducts(options2, specifications2, productPriceRangeMin, productPriceRangeMax,pageNo==0?1:pageNo,4), null);
            //ProductPagedList products = _productService.Search();

            return PartialView(products);
        }
    }
}
