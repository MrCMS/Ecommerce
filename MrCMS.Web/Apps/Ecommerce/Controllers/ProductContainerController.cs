using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class ProductContainerController : MrCMSAppUIController<EcommerceApp>
    {
        public ViewResult Show(ProductContainer productContainer)
        {
            int page = 1;
            if (Request["p"] != null)
            {
                if (!int.TryParse(Request["p"], out page))
                {
                    page = 1;
                }
            }
            return View(new CategoryContainer<Product>(productContainer, null, page));
        }
    }
}
