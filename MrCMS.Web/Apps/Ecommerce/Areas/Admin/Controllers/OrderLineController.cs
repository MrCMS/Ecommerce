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
        public PartialViewResult Add(int orderID)
        {
            ViewData["products"] = _productService.GetOptions();
            OrderLine orderLine = new OrderLine();
            orderLine.Order = _orderService.Get(orderID);
            return PartialView(orderLine);
        }

        [ActionName("Add")]
        [HttpPost]
        public RedirectToRouteResult Add_POST(OrderLine orderLine, int ProductID=0)
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
                    orderLine.TaxRate = product.TaxRatePercentage;
                    orderLine.Order.OrderLines.Add(orderLine);
                    _orderLineService.Save(orderLine);
                }

                return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }

        [HttpGet]
        public PartialViewResult Edit(OrderLine orderLine)
        {
            ViewData["products"] = _productService.GetOptions();
            ViewData["ProductID"] = orderLine.ProductVariant.Id;
            return PartialView(orderLine);
        }

        [ActionName("Edit")]
        [HttpPost]
        public RedirectToRouteResult Edit_POST(OrderLine orderLine, int ProductID)
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
                    orderLine.TaxRate = product.TaxRatePercentage;
                    orderLine.Order.OrderLines.Add(orderLine);
                    _orderLineService.Save(orderLine);
                }

                return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }

        [HttpGet]
        public PartialViewResult Delete(OrderLine orderLine)
        {
            return PartialView(orderLine);
        }

        [ActionName("Delete")]
        [HttpPost]
        public RedirectToRouteResult Delete_POST(OrderLine orderLine)
        {
            if (orderLine.Order != null)
            {
                _orderLineService.Delete(orderLine);
                return RedirectToAction("Edit", "Order", new { id = orderLine.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }
    }
}