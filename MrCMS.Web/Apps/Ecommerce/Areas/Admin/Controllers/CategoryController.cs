using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class EcommerceSearchablePageController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IEcommerceSearchablePageAdminService _ecommerceSearchablePageAdminService;

        public EcommerceSearchablePageController(
            IEcommerceSearchablePageAdminService ecommerceSearchablePageAdminService)
        {
            _ecommerceSearchablePageAdminService = ecommerceSearchablePageAdminService;
        }

        public PartialViewResult HiddenSpecifications(EcommerceSearchablePage searchablePage)
        {
            return PartialView(searchablePage);
        }

        [HttpGet]
        public PartialViewResult AddSpecification(EcommerceSearchablePage searchablePage)
        {
            ViewData["category"] = searchablePage;
            return PartialView(_ecommerceSearchablePageAdminService.GetShownSpecifications(searchablePage));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult AddSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_ecommerceSearchablePageAdminService.AddSpecificationToHidden(attribute, categoryId));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public JsonResult RemoveSpecification(ProductSpecificationAttribute attribute, int categoryId)
        {
            return Json(_ecommerceSearchablePageAdminService.RemoveSpecificationFromHidden(attribute, categoryId));
        }
    }

    public interface IEcommerceSearchablePageAdminService
    {
        List<ProductSpecificationAttribute> GetShownSpecifications(EcommerceSearchablePage category);
        bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId);
        bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId);
    }

    public class EcommerceSearchablePageAdminService : IEcommerceSearchablePageAdminService
    {
        private readonly ISession _session;

        public EcommerceSearchablePageAdminService(ISession session)
        {
            _session = session;
        }

        public List<ProductSpecificationAttribute> GetShownSpecifications(EcommerceSearchablePage category)
        {
            return _session.QueryOver<ProductSpecificationAttribute>()
                .Cacheable()
                .List()
                .Where(attribute => !category.HiddenSearchSpecifications.Contains(attribute))
                .ToList();
        }

        public bool AddSpecificationToHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<EcommerceSearchablePage>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Add(attribute);
            attribute.HiddenInSearchpages.Add(category);
            _session.Transact(session => session.Update(category));
            return true;
        }

        public bool RemoveSpecificationFromHidden(ProductSpecificationAttribute attribute, int categoryId)
        {
            var category = _session.Get<EcommerceSearchablePage>(categoryId);

            if (category == null)
                return false;
            category.HiddenSearchSpecifications.Remove(attribute);
            attribute.HiddenInSearchpages.Remove(category);
            _session.Transact(session => session.Update(category));
            return true;
        }
    }

    public class CategoryController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly ICategoryAdminService _categoryAdminService;

        public CategoryController(ICategoryAdminService categoryAdminService)
        {
            _categoryAdminService = categoryAdminService;
        }

        public ViewResult Index(string q = null, int p = 1)
        {
            if (!_categoryAdminService.ProductContainerExists())
                return View();
            IPagedList<Category> categoryPagedList = _categoryAdminService.Search(q, p);
            return View(categoryPagedList);
        }

        public JsonResult Search(string term, List<int> ids)
        {
            return Json(_categoryAdminService.Search(term, ids ?? new List<int>()));
        }

        [HttpGet]
        public JsonResult SearchCategories(string term)
        {
            if (!string.IsNullOrWhiteSpace(term))
                return
                    Json(
                        _categoryAdminService.Search(term)
                            .Select(x => new {x.Name, CategoryID = x.Id})
                            .Take(15)
                            .ToList());

            return Json(String.Empty, JsonRequestBehavior.AllowGet);
        }
    }
}