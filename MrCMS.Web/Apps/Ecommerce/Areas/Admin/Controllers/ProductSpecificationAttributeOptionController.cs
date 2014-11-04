using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;
using MrCMS.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductSpecificationAttributeOptionController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductOptionManager _productOptionManager;

        public ProductSpecificationAttributeOptionController(IProductOptionManager productOptionManager)
        {
            _productOptionManager = productOptionManager;
        }

        public ActionResult Index(int attributeId=0)
        {
            ProductSpecificationAttribute productSpecificationAttribute = _productOptionManager.GetSpecificationAttribute(attributeId);
            if (productSpecificationAttribute != null)
                return View(productSpecificationAttribute);
            return RedirectToAction("Index","ProductSpecificationAttribute");
        }

        [HttpGet]
        public ActionResult Add(int attributeId = 0)
        {
            ProductSpecificationAttribute productSpecificationAttribute = _productOptionManager.GetSpecificationAttribute(attributeId);
            if (productSpecificationAttribute != null)
                return PartialView(new ProductSpecificationAttributeOption() { ProductSpecificationAttribute=productSpecificationAttribute });
            return RedirectToAction("Index", "ProductSpecificationAttribute");
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Add(ProductSpecificationAttributeOption option)
        {
            if (option.ProductSpecificationAttribute.Id != 0)
            {
                ProductSpecificationAttribute productSpecificationAttribute = _productOptionManager.GetSpecificationAttribute(option.ProductSpecificationAttribute.Id);
                if (productSpecificationAttribute != null)
                {
                    if (ModelState.IsValid)
                    {
                        _productOptionManager.AddSpecificationAttributeOption(option);
                        return RedirectToAction("Index", new { attributeId = option.ProductSpecificationAttribute.Id });
                    }
                    else
                        return PartialView(option);
                }
            }
            return RedirectToAction("Index", "ProductSpecificationAttribute");
        }

        [HttpGet]
        public PartialViewResult Edit(ProductSpecificationAttributeOption option)
        {
            return PartialView(option);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit_POST(ProductSpecificationAttributeOption option)
        {
            if (ModelState.IsValid)
            {
                _productOptionManager.UpdateSpecificationAttributeOption(option);
                return RedirectToAction("Index", new { attributeId = option.ProductSpecificationAttribute.Id });
            }
            else
                return PartialView(option);
        }

        [HttpGet]
        public PartialViewResult Delete(ProductSpecificationAttributeOption option)
        {
            return PartialView(option);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(ProductSpecificationAttributeOption option)
        {
            _productOptionManager.DeleteSpecificationAttributeOption(option);
            return RedirectToAction("Index", new { attributeId = option.ProductSpecificationAttribute.Id });
        }

        [HttpGet]
        public ActionResult Sort(int attributeId=0)
        {
            ProductSpecificationAttribute productSpecificationAttribute = _productOptionManager.GetSpecificationAttribute(attributeId);
            if (productSpecificationAttribute != null)
            {
                var sortItems = productSpecificationAttribute.Options.OrderBy(x => x.DisplayOrder)
                            .Select(
                                arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                            .ToList();
                ViewBag.ProductSpecificationAttribute = productSpecificationAttribute;
                return View(sortItems);
            }
            return RedirectToAction("Index","ProductSpecificationAttribute");
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public ActionResult Sort(List<SortItem> items, int attributeId=0)
        {
            if (attributeId != 0)
            {
                if (items != null && items.Count > 0)
                {
                    _productOptionManager.UpdateSpecificationAttributeOptionDisplayOrder(items);
                }
                return RedirectToAction("Index", new { attributeId = attributeId });
            }
            return RedirectToAction("Index", "ProductSpecificationAttribute");
        }

        public JsonResult IsUniqueAttributeOption(string name, int attributeId=0)
        {
            if (attributeId != 0)
            {
                ProductSpecificationAttribute productSpecificationAttribute = _productOptionManager.GetSpecificationAttribute(attributeId);
                if (productSpecificationAttribute != null)
                {
                    if (_productOptionManager.AnyExistingSpecificationAttributeOptionsWithName(name,productSpecificationAttribute.Id))
                        return Json("There is already an attribute option stored with that name.", JsonRequestBehavior.AllowGet);
                    else
                        return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(String.Empty);
        }
    }
}