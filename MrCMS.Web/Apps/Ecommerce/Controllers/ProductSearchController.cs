using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using NHibernate.Mapping;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Models;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Settings;
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
            decimal productPriceRangeMin= 0;
            decimal productPriceRangeMax=0;
            foreach (var product in (IPagedList<Product>)products)
            {
                if (product.Variants.Count() > 0)
                {
                    foreach (var variant in product.Variants)
                    {
                        if(variant.Price<productPriceRangeMin)
                            productPriceRangeMin=variant.Price;
                        if(variant.Price>productPriceRangeMax)
                            productPriceRangeMax=variant.Price;
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
        public PartialViewResult Results(string sortBy,string options,string specifications, decimal productPriceRangeMin = 0, decimal productPriceRangeMax = 0, int pageNo = 0, int pageSize=0)
        {
            List<string> specs = new List<string>();
            if (!String.IsNullOrWhiteSpace(specifications))
            {
                try
                {
                    string[] rawSpecs = specifications.Split(',');
                    foreach (var item in rawSpecs)
                    {
                        if(item!=String.Empty)
                            specs.Add(item);
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            List<string> ops = new List<string>();
            if (!String.IsNullOrWhiteSpace(options))
            {
                ops.Add(options);
            }

            ProductPagedList products = new ProductPagedList(_productSearchService.SearchProducts(sortBy,
                ops, 
                specs, 
                productPriceRangeMin, 
                productPriceRangeMax, 
                pageNo == 0 ? 1 : pageNo,
                pageSize==0 ? Int32.Parse(MrCMSApplication.Get<EcommerceSettings>().CategoryProductsPerPage.Split(',').First()) : pageSize), null);

            return PartialView(products);
        }
    }
}
