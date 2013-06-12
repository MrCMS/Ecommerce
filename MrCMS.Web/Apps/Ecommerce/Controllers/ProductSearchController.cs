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
namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductSearchController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductService _productService;

        public ProductSearchController(ICategoryService categoryService, IProductOptionManager productOptionManager, IProductService productService)
        {
            _categoryService = categoryService;
            _productOptionManager = productOptionManager;
            _productService = productService;
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
    }
}
