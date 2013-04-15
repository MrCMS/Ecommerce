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
            return View(productContainer);
        }
    }
}
