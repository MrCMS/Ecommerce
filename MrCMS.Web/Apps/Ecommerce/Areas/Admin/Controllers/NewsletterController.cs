using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah.ContentSyndication;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ActionResults;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder.ContentItems;
using MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class NewsletterController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly INewsletterService _newsletterService;
        private readonly INewsletterTemplateService _newsletterTemplateService;
        private readonly IContentItemService _contentItemService;

        public NewsletterController(INewsletterService newsletterService, INewsletterTemplateService newsletterTemplateService, IContentItemService contentItemService)
        {
            _newsletterService = newsletterService;
            _newsletterTemplateService = newsletterTemplateService;
            _contentItemService = contentItemService;
        }

        public ViewResult Index()
        {
            var newsletters = _newsletterService.GetAll().OrderByDescending(x => x.CreatedOn).ToList();

            return View(newsletters);
        }

        // Add

        [HttpGet]
        public PartialViewResult Add()
        {
            ViewData["templates"] = _newsletterTemplateService.GetAll()
                .BuildSelectItemList(x => x.Name, x => x.Id.ToString(), emptyItemText: "Select Template");

            return PartialView();
        }

        [HttpPost]
        public RedirectToRouteResult Add(Newsletter newsletter)
        {
            _newsletterService.Add(newsletter);

            return RedirectToAction("Index");
        }

        // Edit

        [HttpGet]
        public PartialViewResult Edit(Newsletter newsletter)
        {
            ViewData["templates"] = _newsletterTemplateService.GetAll()
                .BuildSelectItemList(x => x.Name, x => x.Id.ToString(), x => x.Id == newsletter.NewsletterTemplate.Id, "Select Template");

            return PartialView(newsletter);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(Newsletter newsletter)
        {
            _newsletterService.Edit(newsletter);

            return RedirectToAction("Index");
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
            _newsletterService.Delete(newsletter);

            return RedirectToAction("Index");
        }

        public ActionResult Preview(Newsletter newsletter)
        {
            return new NewsletterActionResult(newsletter);
        }

        public PartialViewResult ShowContentItems()
        {
            return PartialView();
        }

        // Sort Content Items

        [HttpGet]
        public ActionResult SortContentItems(Newsletter newsletter)
        {
            var contentItems = newsletter.ContentItems;
            var sortItems =
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
                _newsletterService.UpdateContentItemsDisplayOrder(items);
            }
            return RedirectToAction("Index");
        }

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
            _contentItemService.Delete(contentItem);
            var newsletter = contentItem.Newsletter;
            newsletter.ContentItems.Remove(contentItem);
            _newsletterService.Edit(newsletter);

            return RedirectToAction("Edit", new { id = contentItem.Newsletter.Id });
        }

        // Banner

        [HttpGet]
        public PartialViewResult AddBanner(Newsletter newsletter)
        {
            var banner = new Banner { Newsletter = newsletter };
            return PartialView(banner);
        }

        [HttpPost]
        public RedirectToRouteResult AddBanner(Banner banner)
        {
            _contentItemService.Add(banner);
            var newsletter = banner.Newsletter;
            newsletter.ContentItems.Add(banner);
            _newsletterService.Edit(newsletter);
            return RedirectToAction("Edit", new { id = banner.Newsletter.Id });
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
            _contentItemService.Edit(banner);
            return RedirectToAction("Edit", new { id = banner.Newsletter.Id });
        }

        // Free Text

        [HttpGet]
        public PartialViewResult AddFreeText(Newsletter newsletter)
        {
            var freeText = new FreeText { Newsletter = newsletter };

            return PartialView(freeText);
        }

        [HttpPost]
        public RedirectToRouteResult AddFreeText(FreeText freeText)
        {
            _contentItemService.Add(freeText);
            var newsletter = freeText.Newsletter;
            newsletter.ContentItems.Add(freeText);
            _newsletterService.Edit(newsletter);
            return RedirectToAction("Edit", new { id = freeText.Newsletter.Id });
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
            _contentItemService.Edit(freeText);
            return RedirectToAction("Edit", new { id = freeText.Newsletter.Id });
        }

        // Image and Text

        [HttpGet]
        public PartialViewResult AddImageAndText(Newsletter newsletter)
        {
            var imageAndText = new ImageAndText { Newsletter = newsletter };
            return PartialView(imageAndText);
        }

        [HttpPost]
        public RedirectToRouteResult AddImageAndText(ImageAndText imageAndText)
        {
            _contentItemService.Add(imageAndText);
            var newsletter = imageAndText.Newsletter;
            newsletter.ContentItems.Add(imageAndText);
            _newsletterService.Edit(newsletter);

            return RedirectToAction("Edit", new { id = imageAndText.Newsletter.Id });
        }

        [HttpGet]
        public PartialViewResult EditImageAndText(ImageAndText imageAndText)
        {
            return PartialView(imageAndText);
        }

        [HttpPost]
        [ActionName("EditImageAndText")]
        public RedirectToRouteResult EditImageAndText_POST(ImageAndText imageAndText)
        {
            _contentItemService.Edit(imageAndText);
            return RedirectToAction("Edit", new { id = imageAndText.Newsletter.Id });
        }

        // Product List

        [HttpGet]
        public PartialViewResult AddProductList(Newsletter newsletter)
        {
            var productList = new ProductList { Newsletter = newsletter };
            return PartialView(productList);
        }

        [HttpPost]
        public RedirectToRouteResult AddProductList(ProductList productList)
        {
            _contentItemService.Add(productList);
            var newsletter = productList.Newsletter;
            newsletter.ContentItems.Add(productList);
            _newsletterService.Edit(newsletter);

            return RedirectToAction("Edit", new { id = productList.Newsletter.Id });
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
            _contentItemService.Edit(productList);
            return RedirectToAction("Edit", new { id = productList.Newsletter.Id });
        }
    }
}