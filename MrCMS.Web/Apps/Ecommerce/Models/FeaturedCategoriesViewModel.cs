using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class FeaturedCategoriesViewModel
    {
        public FeaturedCategoriesViewModel()
        {
            Categories = new List<Category>();
        }
        public List<Category> Categories { get; set; }
        public string Title { get; set; }
    }
}