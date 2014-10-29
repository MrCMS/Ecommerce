using System;
using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Stock.Entities;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public interface ICSVFileWriter
    {
        byte[] GetFile<T>(IEnumerable<T> items, Dictionary<string, Func<T, object>> columns);
    }
}