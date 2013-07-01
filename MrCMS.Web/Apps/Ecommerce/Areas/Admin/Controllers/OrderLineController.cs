using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderLineController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderLineService _orderLineService;
        private readonly IOrderService _orderService;
        private readonly IProductVariantService _productVariantService;

        public OrderLineController(IOrderLineService orderLineService, IOrderService orderService, IProductVariantService productVariantService)
        {
            _orderLineService = orderLineService;
            _orderService = orderService;
            _productVariantService = productVariantService;
        }

        [HttpGet]
        public ViewResult Edit(OrderLine orderLine)
        {
            ViewData["products"] = _productVariantService.GetOptions();
            ViewData["ProductID"] = orderLine.ProductVariant.Id;
            return View(orderLine);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(OrderLine orderLine, int variantId)
        {
            if (orderLine.Order != null)
            {
                var productVariant = _productVariantService.Get(variantId);
                if (productVariant.CanBuy(orderLine.Quantity))
                {
                    orderLine.ProductVariant = productVariant;
                    orderLine.Subtotal = orderLine.Quantity * productVariant.PricePreTax;
                    orderLine.Weight = orderLine.Quantity * productVariant.Weight;
                    orderLine.UnitPrice = productVariant.Price;
                    orderLine.Tax = orderLine.Quantity * productVariant.Tax;
                    orderLine.TaxRate = productVariant.TaxRate.Percentage;
                    orderLine.Order.OrderLines.Add(orderLine);
                }
                else
                {
                    ModelState.AddModelError("Quantity", "Requested Quantity is not available for purchase. "+(productVariant.StockRemaining.HasValue?"Stock remaining:"+productVariant.StockRemaining.Value:String.Empty));
                }

                if (orderLine.Discount > (orderLine.Quantity * productVariant.PricePreTax))
                {
                    ModelState.AddModelError("Discount", "Discount value cannot be greater than Subtotal. (" + (orderLine.Quantity * productVariant.PricePreTax) + ")");
                }

                if (ModelState.IsValid)
                {
                    _orderLineService.Save(orderLine);
                    return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
                }
                else
                {
                    ViewData["products"] = _productVariantService.GetOptions();
                    ViewData["ProductID"] = orderLine.ProductVariant.Id;
                    return View(orderLine);
                }
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }
    }
}