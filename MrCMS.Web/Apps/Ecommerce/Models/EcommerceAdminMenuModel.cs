using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private readonly EcommerceSettings _ecommerceSettings;
        private readonly UrlHelper _urlHelper;

        public string Text
        {
            get { return "Catalog"; }
        }

        public string IconClass
        {
            get { return "fa fa-shopping-cart"; }
        }

        public string Url { get; private set; }

        public bool CanShow
        {
            get { return CurrentRequestData.CurrentUser.CanAccess<CatalogAdminMenuACL>(CatalogAdminMenuACL.ShowMenu); }
        }

        public SubMenu Children
        {
            get
            {
                var adminItems = new List<ChildMenuItem>
                {
                    new ChildMenuItem("Orders", _urlHelper.Action("Index", "Order"), ACLOption.Create(new OrderACL(), OrderACL.List)),
                    new ChildMenuItem("Products", _urlHelper.Action("Index", "Product"), ACLOption.Create(new ProductACL(), ProductACL.List)),
                    new ChildMenuItem("Categories", _urlHelper.Action("Index", "Category"), ACLOption.Create(new CategoryACL(), CategoryACL.List)),
                    new ChildMenuItem("Brands", _urlHelper.Action("Index", "Brand"), ACLOption.Create(new BrandACL(), BrandACL.List)),
                    new ChildMenuItem("Product Specifications", _urlHelper.Action("Index", "ProductSpecificationAttribute"), ACLOption.Create(new ProductSpecificationAttributeACL(), ProductSpecificationAttributeACL.List)),
                    new ChildMenuItem("Sort Option Values", _urlHelper.Action("Index", "ProductOptionSorting"), ACLOption.Create(new ProductOptionSortingACL(), ProductOptionSortingACL.List)),
                    new ChildMenuItem("Discounts", _urlHelper.Action("Index", "Discount"), ACLOption.Create(new DiscountACL(), DiscountACL.List)),
                    new ChildMenuItem("E-Tags", _urlHelper.Action("Index", "ETag"), ACLOption.Create(new ETagACL(), ETagACL.List)),
                    new ChildMenuItem("Customers Carts", _urlHelper.Action("Index", "OnlineCustomers"), ACLOption.Create(new OnlineCustomersACL(), OnlineCustomersACL.ViewCustomers))
                };

                if (_ecommerceSettings.GiftCardsEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Gift Cards", _urlHelper.Action("Index", "GiftCard"),
                        ACLOption.Create(new GiftCardACL(), GiftCardACL.List)));
                }

                if (_ecommerceSettings.WarehouseStockEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Warehouses", _urlHelper.Action("Index", "Warehouse"),
                        ACLOption.Create(new WarehouseACL(), WarehouseACL.List)));
                    adminItems.Add(new ChildMenuItem("Warehouse Stock", _urlHelper.Action("Index", "WarehouseStock"),
                        ACLOption.Create(new WarehouseStockACL(), WarehouseStockACL.List)));
                }

                var ecommerceMenu = new SubMenu();
                ecommerceMenu.AddRange(adminItems);
                ecommerceMenu.Add(new ChildMenuItem("Product Reviews", _urlHelper.Action("Index", "ProductReview"), ACLOption.Create(new ProductReviewACL(), ProductReviewACL.List)));
                return ecommerceMenu;
            }
        }

        public int DisplayOrder
        {
            get { return 45; }
        }

        public EcommerceAdminMenuModel(EcommerceSettings ecommerceSettings, UrlHelper urlHelper)
        {
            _ecommerceSettings = ecommerceSettings;
            _urlHelper = urlHelper;
        }
    }
}