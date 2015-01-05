using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.ModelBinders
{
    public class ProductSortDataModelBinder : MrCMSDefaultModelBinder
    {
        public ProductSortDataModelBinder(IKernel kernel)
            : base(kernel)
        {
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var idKeys = controllerContext.HttpContext.Request.Form.AllKeys.Where(s => s.StartsWith("id-"));
            var productSortDatas = new List<ProductSortData>();
            foreach (var idKey in idKeys)
            {
                var idVal = controllerContext.GetValueFromRequest(idKey);
                var orderVal = controllerContext.GetValueFromRequest("order-" + idKey.Substring(3));
                int id, order;
                if (int.TryParse(idVal, out id) && int.TryParse(orderVal, out order))
                {
                    productSortDatas.Add(new ProductSortData
                    {
                        Id = id,
                        DisplayOrder = order
                    });
                }
            }
            return productSortDatas;
        }
    }
}