using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.Categories;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IBrandService _brandService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentService _documentService;
        private readonly IFileAdminService _fileAdminService;
        private readonly IProductOptionManagementService _productOptionManagementService;
        private readonly IProductOptionManager _productOptionManager;
        private readonly IProductService _productService;
        private readonly SiteSettings _siteSettings;
        private readonly IUniquePageService _uniquePageService;

        public ProductController(IProductService productService, IDocumentService documentService,
            ICategoryService categoryService,
            IProductOptionManager productOptionManager, IFileAdminService fileAdminService, IBrandService brandService,
            IProductOptionManagementService productOptionManagementService, SiteSettings siteSettings,
            IUniquePageService uniquePageService)
        {
            _productService = productService;
            _documentService = documentService;
            _categoryService = categoryService;
            _productOptionManager = productOptionManager;
            _fileAdminService = fileAdminService;
            _brandService = brandService;
            _productOptionManagementService = productOptionManagementService;
            _siteSettings = siteSettings;
            _uniquePageService = uniquePageService;
        }

        /// <summary>
        ///     Lists products
        /// </summary>
        /// <param name="q">query string to filter by</param>
        /// <param name="p">page number</param>
        /// <returns></returns>
        /// [MrCMSACLRule(typeof(ProductACL), ProductACL.List)]
        [MrCMSACLRule(typeof(ProductACL), ProductACL.List)]
        public ViewResult Index(ProductAdminSearchQuery searchQuery)
        {
            ViewData["publish-status"] = _productService.GetPublishStatusOptions();
            ViewData["results"] = _productService.Search(searchQuery);
            var productContainer = _uniquePageService.GetUniquePage<ProductContainer>();
            ViewData["product-containerId"] = productContainer == null ? (int?)null : productContainer.Id;

            return View(searchQuery);
        }
        
        [HttpGet]
        public PartialViewResult Categories(Product product)
        {
            return PartialView(product);
        }

        [HttpGet]
        public PartialViewResult AddCategory(Product product, string query, int page = 1)
        {
            ViewData["product"] = product;
            IPagedList<Category> categories = _categoryService.GetCategories(product, query, page);
            return PartialView(categories);
        }

        [HttpGet]
        public PartialViewResult AddCategoryCategories(Product product, string query, int page = 1)
        {
            ViewData["product"] = product;
            IPagedList<Category> categories = _categoryService.GetCategories(product, query, page);
            return PartialView(categories);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddCategory(Product product, int categoryId)
        {
            try
            {
                _productService.AddCategory(product, categoryId);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult SearchCategories(Product product, string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(_categoryService.GetCategories(product, String.Empty, 1)
                    .Select(x => new { x.Name, CategoryID = x.Id }).Take(_siteSettings.DefaultPageSize).ToList());

            return Json(_categoryService.GetCategories(product, term, 1)
                .Select(x => new { x.Name, CategoryID = x.Id }).Take(_siteSettings.DefaultPageSize).ToList(),
                JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public PartialViewResult RemoveCategory(Product product, int categoryId)
        {
            ViewData["category"] = _documentService.GetDocument<Category>(categoryId);
            return PartialView(product);
        }

        [HttpPost]
        [ActionName("RemoveCategory")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult RemoveCategory_POST(Product product, int categoryId)
        {
            _productService.RemoveCategory(product, categoryId);

            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpGet]
        public PartialViewResult AddSpecification(Product product)
        {
            ViewData["product"] = product;
            List<ProductSpecificationAttribute> attributes =
                _productOptionManager.ListSpecificationAttributes()
                    .ToList()
                    .Where(
                        x =>
                            product.SpecificationValues.All(
                                v =>
                                    v.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Id !=
                                    x.Id))
                    .ToList();

            ViewData["specification-attributes"] = new SelectList(attributes, "Id", "Name");
            List<ProductSpecificationAttributeOption> options = attributes.Any()
                ? attributes.First().Options.OrderBy(x => x.DisplayOrder).ToList()
                : new List<ProductSpecificationAttributeOption>();
            options.Add(new ProductSpecificationAttributeOption { Id = 0, Name = "Other" });
            ViewData["specification-attributes-options"] = new SelectList(options, "Id", "Name");
            return PartialView(new ProductSpecificationValue { Product = product });
        }

        [HttpGet]
        public JsonResult GetSpecificationAttributeOptions(int specificationAttributeId = 0)
        {
            try
            {
                List<SelectListItem> options = _productOptionManager.GetSpecificationAttribute(specificationAttributeId)
                    .Options.OrderBy(x => x.DisplayOrder)
                    .ToList()
                    .Select(item => new SelectListItem { Selected = false, Text = item.Name, Value = item.Id.ToString() })
                    .ToList();
                return Json(options);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddSpecification(string value, int option = 0, int productId = 0)
        {
            if (!String.IsNullOrWhiteSpace(value) && option != 0 && productId != 0)
            {
                ProductSpecificationAttribute attribute = _productOptionManager.GetSpecificationAttribute(option);
                if (_productOptionManager.ListSpecificationAttributeOptions(option).All(x => x.Name != value))
                    _productOptionManager.AddSpecificationAttributeOption(new ProductSpecificationAttributeOption
                                                                          {
                                                                              Name = value,
                                                                              ProductSpecificationAttribute = attribute
                                                                          });
                _productOptionManager.SetSpecificationValue(_productService.Get(productId), attribute, value);
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public PartialViewResult RemoveSpecification(Product product, int specificationValueId)
        {
            ViewData["specification-value"] = _productOptionManager.GetSpecificationValue(specificationValueId);
            return PartialView(product);
        }

        [HttpPost]
        [ActionName("RemoveSpecification")]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult RemoveSpecification_POST(Product product, int specificationValueId)
        {
            _productOptionManager.DeleteSpecificationValue(
                _productOptionManager.GetSpecificationValue(specificationValueId));

            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpGet]
        public ActionResult SortSpecifications(int productId = 0)
        {
            if (productId != 0)
            {
                Product product = _productService.Get(productId);
                if (product != null)
                {
                    List<SortItem> sortItems = product.SpecificationValues.OrderBy(x => x.DisplayOrder)
                        .Select(
                            arg =>
                                new SortItem
                                {
                                    Order = arg.DisplayOrder,
                                    Id = arg.Id,
                                    Name = arg.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name
                                })
                        .ToList();
                    ViewBag.Product = product;
                    return View(sortItems);
                }
            }
            return RedirectToAction("Edit", "Webpage", new { id = productId });
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult SortSpecifications(List<SortItem> items, int productId = 0)
        {
            if (productId != 0)
            {
                if (items != null && items.Count > 0)
                {
                    _productOptionManager.UpdateSpecificationValueDisplayOrder(items);
                }
                return RedirectToAction("Edit", "Webpage", new { id = productId });
            }
            return RedirectToAction("Edit", "Webpage", new { id = productId });
        }

        public PartialViewResult PricingButtons(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult PricingInfo(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult Specifications(Product product)
        {
            return PartialView(product);
        }

        public PartialViewResult Images(Product product)
        {
            return PartialView(product);
        }

        [HttpGet]
        public ActionResult SortImages(Product product)
        {
            ViewBag.Product = product;
            List<ImageSortItem> sortItems =
                _fileAdminService.GetFiles(product.Gallery).OrderBy(arg => arg.display_order)
                    .Select(
                        arg => new ImageSortItem
                               {
                                   Order = arg.display_order,
                                   Id = arg.Id,
                                   Name = arg.name,
                                   ImageUrl = arg.url,
                                   IsImage = arg.is_image
                               })
                    .ToList();

            return View(sortItems);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult SortImages(int productId, List<SortItem> items)
        {
            _fileAdminService.SetOrders(items);
            return RedirectToAction("Edit", "Webpage", new { id = productId });
        }


        [HttpGet]
        public ActionResult SortOptions(Product product)
        {
            if (product != null)
            {
                List<SortItem> sortItems = product.Options
                    .Select((option, i) =>
                        new SortItem { Order = i, Id = option.Id, Name = option.Name })
                    .ToList();
                ViewBag.Product = product;
                return View(sortItems);
            }
            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult SortOptions(List<SortItem> items, Product product)
        {
            _productOptionManager.UpdateAttributeOptionDisplayOrder(product, items);
            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpGet]
        public PartialViewResult ManageOptions(Product product)
        {
            return PartialView(product);
        }

        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
                return
                    Json(
                        _productService.Search(term)
                            .Select(x => new { x.Name, ProductID = x.Id })
                            .Take(_siteSettings.DefaultPageSize)
                            .ToList());

            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Brands(Product product)
        {
            ViewData["brands"] = _brandService.GetOptions();
            return PartialView(product);
        }

        [HttpGet]
        public PartialViewResult AddBrand(Product product)
        {
            ViewData["product"] = product;
            return PartialView(new Brand());
        }

        [HttpPost]
        [ActionName("AddBrand")]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddBrand_POST(Brand brand)
        {
            _brandService.Add(brand);
            return Json(brand.Id);
        }

        [HttpGet]
        public ActionResult SortCategories(Product product)
        {
            IList<Category> categories = product.Categories;
            List<SortItem> sortItems = categories
                .Select(
                    arg =>
                        new SortItem
                        {
                            Order = categories.IndexOf(arg),
                            Id = arg.Id,
                            Name = arg.Name
                        })
                .ToList();
            ViewBag.Product = product;
            return View(sortItems);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult SortCategories(Product product, List<SortItem> items)
        {
            _productService.SetCategoryOrder(product, items);
            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpGet]
        public PartialViewResult AddProductOption(Product product)
        {
            ViewData["options"] = _productOptionManagementService.GetProductAttributeOptions(product);
            return PartialView(product);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddProductOption(Product product,
            [IoCModelBinder(typeof(ProductOptionModelBinder))] ProductOption productOption)
        {
            _productOptionManagementService.AddOption(product, productOption);
            return Json(true);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult RemoveProductOption(Product product,
            [IoCModelBinder(typeof(ProductOptionModelBinder))] ProductOption productOption)
        {
            _productOptionManagementService.RemoveOption(product, productOption);
            return Json(true);
        }

        public PartialViewResult ProductOptions(Product product)
        {
            return PartialView(product);
        }

        //Related Products

        [HttpGet]
        public PartialViewResult RelatedProducts(Product product)
        {
            return PartialView(product);
        }

        [HttpGet]
        public PartialViewResult AddRelatedProduct(Product product, string query, int page = 1)
        {
            ViewData["product"] = product;
            IPagedList<Product> items = _productService.RelatedProductsSearch(product, query, page);
            return PartialView(items);
        }

        [HttpGet]
        public PartialViewResult AddRelatedProductItems(Product product, string query, int page = 1)
        {
            ViewData["product"] = _documentService.GetDocument<Product>(product.Id);
            IPagedList<Product> items = _productService.RelatedProductsSearch(product, query, page);
            return PartialView(items);
        }

        [HttpPost]
        public JsonResult AddRelatedProduct(Product product, int relatedProductId)
        {
            try
            {
                _productService.AddRelatedProduct(product, relatedProductId);
                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpGet]
        public PartialViewResult RemoveRelatedProduct(Product product, int relatedProductId)
        {
            ViewData["related-product"] = product.RelatedProducts.SingleOrDefault(x => x.Id == relatedProductId);
            return PartialView(product);
        }

        [HttpPost]
        [ActionName("RemoveRelatedProduct")]
        public RedirectToRouteResult RemoveRelatedProduct_POST(Product product, int relatedProductId)
        {
            Product relatedProduct = product.RelatedProducts.SingleOrDefault(x => x.Id == relatedProductId);
            product.RelatedProducts.Remove(relatedProduct);
            _documentService.SaveDocument(relatedProduct);

            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }

        [HttpGet]
        public PartialViewResult SortVariants(Product product)
        {
            IList<ProductVariant> variants = product.Variants;
            var sortItems = new List<SortItem>();
            foreach (ProductVariant variant in variants)
            {
                string name = !string.IsNullOrEmpty(variant.Name) ? variant.Name : product.Name;

                if (variant.OptionValues.Any())
                {
                    name += string.Format(" {0}", string.Join(" - ", variant.OptionValues.Select(value => value.Value)));
                }
                sortItems.Add(new SortItem
                              {
                                  Id = variant.Id,
                                  Name = name,
                                  Order = product.Variants.IndexOf(variant)
                              });
            }
            ViewData["sort-items"] = sortItems;
            return PartialView(product);
        }

        [HttpPost]
        public RedirectToRouteResult SortVariants(Product product, List<SortItem> items)
        {
            _productService.SetVariantOrders(product, items);
            return RedirectToAction("Edit", "Webpage", new { id = product.Id });
        }
    }
}