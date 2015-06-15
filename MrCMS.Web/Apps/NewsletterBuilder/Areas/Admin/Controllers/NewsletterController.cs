using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.NewsletterBuilder.ActionResults;
using MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;
using MrCMS.Web.Apps.NewsletterBuilder.Entities.ContentItems;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Controllers
{
    public class NewsletterController : MrCMSAppAdminController<NewsletterBuilderApp>
    {
        private readonly INewsletterAdminService _newsletterAdminService;

        public NewsletterController(INewsletterAdminService newsletterAdminService)
        {
            _newsletterAdminService = newsletterAdminService;
        }

        public ViewResult Index()
        {
            List<Newsletter> newsletters = _newsletterAdminService.GetAll().OrderByDescending(x => x.CreatedOn).ToList();
            return View(newsletters);
        }

        // Add

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewData["templates"] = _newsletterAdminService.GetTemplateOptions();
            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(Newsletter newsletter)
        {
            _newsletterAdminService.Add(newsletter);
            return RedirectToAction("Edit", new {id = newsletter.Id});
        }

        // Edit

        [HttpGet]
        public PartialViewResult Edit(Newsletter newsletter)
        {
            ViewData["templates"] = _newsletterAdminService.GetTemplateOptions();
            ViewData["content-item-types"] = _newsletterAdminService.GetContentItemTypes();
            return PartialView(newsletter);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(Newsletter newsletter)
        {
            _newsletterAdminService.Edit(newsletter);
            return RedirectToAction("Edit", new {id = newsletter.Id});
        }

        // Delete

        [HttpGet]
        public PartialViewResult Delete(Newsletter newsletter)
        {
            return PartialView(newsletter);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(Newsletter newsletter)
        {
            _newsletterAdminService.Delete(newsletter);

            return RedirectToAction("Index");
        }

        public ActionResult Preview(Newsletter newsletter)
        {
            return new NewsletterActionResult(newsletter);
        }


        // Sort Content Items

        [HttpGet]
        public ActionResult SortContentItems(Newsletter newsletter)
        {
            IList<ContentItem> contentItems = newsletter.ContentItems;
            List<SortItem> sortItems =
                contentItems.OrderBy(x => x.DisplayOrder)
                    .Select(
                        arg =>
                            new SortItem
                            {
                                Order = arg.DisplayOrder,
                                Id = arg.Id,
                                Name = arg.Name + " (" + arg.GetType().Name + ")"
                            }).ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult SortContentItems(List<SortItem> items)
        {
            if (items != null && items.Count > 0)
            {
                _newsletterAdminService.UpdateContentItemsDisplayOrder(items);
            }
            return RedirectToAction("Index");
        }

        public RedirectToRouteResult Clone(Newsletter newsletter)
        {
            var cloneOne = _newsletterAdminService.Clone(newsletter);
            TempData["clone-success"] = true;
            return RedirectToAction("Edit", new { id = cloneOne.Id });
        }

        /*
        // Content Items

        [HttpGet]
        public PartialViewResult DeleteContentItem(ContentItem contentItem)
        {
            return PartialView();
        }

        [HttpPost]
        [ActionName("DeleteContentItem")]
        public RedirectToRouteResult DeleteContentItem_POST(ContentItem contentItem)
        {
            _contentItemAdminService.Delete(contentItem);
            Newsletter newsletter = contentItem.Newsletter;
            newsletter.ContentItems.Remove(contentItem);
            _newsletterAdminService.Edit(newsletter);

            return RedirectToAction("Edit", new {id = contentItem.Newsletter.Id});
        }

        // Banner

        [HttpGet]
        public PartialViewResult AddBanner(Newsletter newsletter)
        {
            var banner = new Banner {Newsletter = newsletter};
            return PartialView(banner);
        }

        [HttpPost]
        public RedirectToRouteResult AddBanner(Banner banner)
        {
            _contentItemAdminService.Add(banner);
            Newsletter newsletter = banner.Newsletter;
            newsletter.ContentItems.Add(banner);
            _newsletterAdminService.Edit(newsletter);
            return RedirectToAction("Edit", new {id = banner.Newsletter.Id});
        }

        [HttpGet]
        public PartialViewResult EditBanner(Banner banner)
        {
            return PartialView(banner);
        }

        [HttpPost]
        [ActionName("EditBanner")]
        public RedirectToRouteResult EditBanner_POST(Banner banner)
        {
            _contentItemAdminService.Update(banner);
            return RedirectToAction("Edit", new {id = banner.Newsletter.Id});
        }

        // Free Text

        [HttpGet]
        public PartialViewResult AddFreeText(Newsletter newsletter)
        {
            var freeText = new FreeText {Newsletter = newsletter};

            return PartialView(freeText);
        }

        [HttpPost]
        public RedirectToRouteResult AddFreeText(FreeText freeText)
        {
            _contentItemAdminService.Add(freeText);
            Newsletter newsletter = freeText.Newsletter;
            newsletter.ContentItems.Add(freeText);
            _newsletterAdminService.Edit(newsletter);
            return RedirectToAction("Edit", new {id = freeText.Newsletter.Id});
        }

        [HttpGet]
        public PartialViewResult EditFreeText(FreeText freeText)
        {
            return PartialView(freeText);
        }

        [HttpPost]
        [ActionName("EditFreeText")]
        public RedirectToRouteResult EditFreeText_POST(FreeText freeText)
        {
            _contentItemAdminService.Update(freeText);
            return RedirectToAction("Edit", new {id = freeText.Newsletter.Id});
        }

        // Image and Text Left

        [HttpGet]
        public PartialViewResult AddImageRightAndTextLeft(Newsletter newsletter)
        {
            var imageAndText = new ImageRightAndTextLeft {Newsletter = newsletter};
            return PartialView(imageAndText);
        }

        [HttpPost]
        public RedirectToRouteResult AddImageRightAndTextLeft(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            _contentItemAdminService.Add(imageRightAndTextLeft);
            Newsletter newsletter = imageRightAndTextLeft.Newsletter;
            newsletter.ContentItems.Add(imageRightAndTextLeft);
            _newsletterAdminService.Edit(newsletter);

            return RedirectToAction("Edit", new {id = imageRightAndTextLeft.Newsletter.Id});
        }

        [HttpGet]
        public PartialViewResult EditImageRightAndTextLeft(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            return PartialView(imageRightAndTextLeft);
        }

        [HttpPost]
        [ActionName("EditImageRightAndTextLeft")]
        public RedirectToRouteResult EditImageRightAndTextLeft_POST(ImageRightAndTextLeft imageRightAndTextLeft)
        {
            _contentItemAdminService.Update(imageRightAndTextLeft);
            return RedirectToAction("Edit", new {id = imageRightAndTextLeft.Newsletter.Id});
        }

        // Image and Text Right

        [HttpGet]
        public PartialViewResult AddImageLeftAndTextRight(Newsletter newsletter)
        {
            var imageAndText = new ImageLeftAndTextRight {Newsletter = newsletter};
            return PartialView(imageAndText);
        }

        [HttpPost]
        public RedirectToRouteResult AddImageLeftAndTextRight(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            _contentItemAdminService.Add(imageLeftAndTextRight);
            Newsletter newsletter = imageLeftAndTextRight.Newsletter;
            newsletter.ContentItems.Add(imageLeftAndTextRight);
            _newsletterAdminService.Edit(newsletter);

            return RedirectToAction("Edit", new {id = imageLeftAndTextRight.Newsletter.Id});
        }

        [HttpGet]
        public PartialViewResult EditImageLeftAndTextRight(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            return PartialView(imageLeftAndTextRight);
        }

        [HttpPost]
        [ActionName("EditImageLeftAndTextRight")]
        public RedirectToRouteResult EditImageLeftAndTextRight_POST(ImageLeftAndTextRight imageLeftAndTextRight)
        {
            _contentItemAdminService.Update(imageLeftAndTextRight);
            return RedirectToAction("Edit", new {id = imageLeftAndTextRight.Newsletter.Id});
        }

        // Product List

        [HttpGet]
        public PartialViewResult AddProductList(Newsletter newsletter)
        {
            var productList = new ProductList {Newsletter = newsletter};
            return PartialView(productList);
        }

        [HttpPost]
        public RedirectToRouteResult AddProductList(ProductList productList)
        {
            _contentItemAdminService.Add(productList);
            Newsletter newsletter = productList.Newsletter;
            newsletter.ContentItems.Add(productList);
            _newsletterAdminService.Edit(newsletter);

            return RedirectToAction("Edit", new {id = productList.Newsletter.Id});
        }

        [HttpGet]
        public PartialViewResult EditProductList(ProductList productList)
        {
            return PartialView(productList);
        }

        [HttpPost]
        [ActionName("EditProductList")]
        public RedirectToRouteResult EditProductList_POST(ProductList productList)
        {
            _contentItemAdminService.Update(productList);
            return RedirectToAction("Edit", new {id = productList.Newsletter.Id});
        }*/
    }
}