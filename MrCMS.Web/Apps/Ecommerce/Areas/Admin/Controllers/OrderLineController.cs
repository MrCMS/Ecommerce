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
        private readonly IProductService _productService;

        public OrderLineController(IOrderLineService orderLineService, IOrderService orderService, IProductService productService)
        {
            _orderLineService = orderLineService;
            _orderService = orderService;
            _productService = productService;
        }

        [HttpGet]
        public ViewResult Edit(OrderLine orderLine)
        {
            ViewData["products"] = _productService.GetOptions();
            ViewData["ProductID"] = orderLine.ProductVariant.Id;
            return View(orderLine);
        }

        [ActionName("Edit")]
        [HttpPost]
        public ActionResult Edit_POST(OrderLine orderLine, int ProductID)
        {
            if (orderLine.Order != null)
            {
                Product product = _productService.Get(ProductID);
                if (product.CanBuy(orderLine.Quantity))
                {
                    orderLine.ProductVariant = product;
                    orderLine.Subtotal = orderLine.Quantity * product.PricePreTax;
                    orderLine.Weight = orderLine.Quantity * product.Weight;
                    orderLine.UnitPrice = product.Price;
                    orderLine.Tax = orderLine.Quantity * product.Tax;
                    orderLine.TaxRate = product.TaxRate.Percentage;
                    orderLine.Order.OrderLines.Add(orderLine);
                }
                else
                {
                    ModelState.AddModelError("Quantity", "Requested Quantity is not available for purchase. "+(product.StockRemaining.HasValue?"Stock remaining:"+product.StockRemaining.Value:String.Empty));
                }

                if (orderLine.Discount > (orderLine.Quantity * product.PricePreTax))
                {
                    ModelState.AddModelError("Discount", "Discount value cannot be greater than Subtotal. (" + (orderLine.Quantity * product.PricePreTax) + ")");
                }

                if (ModelState.IsValid)
                {
                    _orderLineService.Save(orderLine);
                    return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
                }
                else
                {
                    ViewData["products"] = _productService.GetOptions();
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