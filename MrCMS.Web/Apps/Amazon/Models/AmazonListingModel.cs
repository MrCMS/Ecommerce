using System;
using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonListingModel
    {
        public AmazonListingModel()
        {
            Name = String.Empty;
            Page = 1;
            PageSize = 10;

            Products = new PagedList<Product>(new List<Product>(), Page, PageSize);

            //Categories = new PagedList<AmazonCategory>(new List<AmazonCategory>(), Page, PageSize);
            MultipleVariations = true;
        }

        //Listing
        public Product ChosenProduct { get; set; }
        //public AmazonCategory ChosenCategory { get; set; }

        //Search & Paging
        public string Name { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        //Products
        public IPagedList<Product> Products { get; set; }

        //Categories
        //public IPagedList<AmazonCategory> Categories { get; set; }
        public bool MultipleVariations { get; set; }
    }

}