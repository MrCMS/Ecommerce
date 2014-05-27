using System.Collections.Generic;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.NewsletterBuilder;

namespace MrCMS.Web.Apps.Ecommerce.Services.NewsletterBuilder
{
    public interface INewsletterService
    {
        IList<Newsletter> GetAll();
        void Add(Newsletter newsletter);
        void Edit(Newsletter newsletter);
        void Delete(Newsletter newsletter);
        void UpdateContentItemsDisplayOrder(List<SortItem> items);
    }
}