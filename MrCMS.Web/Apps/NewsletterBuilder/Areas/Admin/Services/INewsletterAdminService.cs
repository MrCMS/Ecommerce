using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Models;
using MrCMS.Web.Apps.NewsletterBuilder.Entities;

namespace MrCMS.Web.Apps.NewsletterBuilder.Areas.Admin.Services
{
    public interface INewsletterAdminService
    {
        IList<Newsletter> GetAll();
        void Add(Newsletter newsletter);
        void Edit(Newsletter newsletter);
        void Delete(Newsletter newsletter);
        void UpdateContentItemsDisplayOrder(List<SortItem> items);
        List<SelectListItem> GetTemplateOptions();
        HashSet<Type> GetContentItemTypes();
        Newsletter Clone(Newsletter newsletter);
    }
}