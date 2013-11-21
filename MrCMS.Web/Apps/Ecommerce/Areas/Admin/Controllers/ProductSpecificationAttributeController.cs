using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class ProductSpecificationAttributeController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IProductOptionManager _productOptionManager;

        public ProductSpecificationAttributeController(IProductOptionManager productOptionManager)
        {
            _productOptionManager = productOptionManager;
        }

        public ViewResult Index()
        {
            var options = _productOptionManager.ListSpecificationAttributes();
            return View(options);
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Add(ProductSpecificationAttribute option)
        {
            if (ModelState.IsValid)
            {
                _productOptionManager.AddSpecificationAttribute(option);
                return RedirectToAction("Index");
            }
            else
            {
                return PartialView(option);
            }
        }

        [HttpGet]
        public PartialViewResult Edit(ProductSpecificationAttribute option)
        {
            return PartialView(option);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(ProductSpecificationAttribute option)
        {
            if (ModelState.IsValid)
            {
                _productOptionManager.UpdateSpecificationAttribute(option);
                return RedirectToAction("Index");
            }
            else
            {
                return PartialView(option);
            }
        }

        [HttpGet]
        public PartialViewResult Delete(ProductSpecificationAttribute option)
        {
            return PartialView(option);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(ProductSpecificationAttribute option)
        {
            _productOptionManager.DeleteSpecificationAttribute(option);
            return RedirectToAction("Index");
        }

        public JsonResult IsUniqueAttribute(string name)
        {
            if (_productOptionManager.AnyExistingSpecificationAttributesWithName(name))
                    return Json("There is already an attribute stored with that name.", JsonRequestBehavior.AllowGet);
            else
                    return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Sort()
        {
            var options = _productOptionManager.ListSpecificationAttributes();
            var sortItems = options.OrderBy(x => x.DisplayOrder)
                                   .Select(
                                       arg => new SortItem {Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name})
                                   .ToList();
            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(List<SortItem> items)
        {
            if (items != null && items.Count > 0)
            {
                _productOptionManager.UpdateSpecificationAttributeDisplayOrder(items);
            }
            return RedirectToAction("Index", "ProductSpecificationAttribute");
        }
    }
}