using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Ecommerce.Services.Products
{
    public interface IImportExportManager
    {
        byte[] ExportProductsToExcel();
        List<string> ImportProductsFromExcel(HttpPostedFileBase file);
    }
}