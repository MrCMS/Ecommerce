using System.Collections.Generic;
using System.Web.UI.WebControls;
using MarketplaceWebServiceFeedsClasses;
using MrCMS.Models;
using MrCMS.Web.Apps.Ecommerce.ACL;
using MrCMS.Web.Apps.Ecommerce.Services;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class EcommerceAdminMenuModel : IAdminMenuItem
    {
        private readonly IShippingMethodSubmenuGenerator _shippingMethodSubmenuGenerator;
        private readonly EcommerceSettings _ecommerceSettings;

        public EcommerceAdminMenuModel(IShippingMethodSubmenuGenerator shippingMethodSubmenuGenerator,
             EcommerceSettings ecommerceSettings)
        {
            _shippingMethodSubmenuGenerator = shippingMethodSubmenuGenerator;
            _ecommerceSettings = ecommerceSettings;
        }

        public string Text
        {
            get { return "Catalog"; }
        }

        public string IconClass { get { return "fa fa-shopping-cart"; } }

        public string Url { get; private set; }

        public bool CanShow
        {
            get { return CurrentRequestData.CurrentUser.CanAccess<CatalogACL>(CatalogACL.Show); }
        }

        public SubMenu Children
        {
            get
            {
                var adminItems = new List<ChildMenuItem>
                {
                    new ChildMenuItem("Orders", "/Admin/Apps/Ecommerce/Order",
                        ACLOption.Create(new OrderACL(), OrderACL.List)),
                    new ChildMenuItem("Products", "/Admin/Apps/Ecommerce/Product",
                        ACLOption.Create(new ProductACL(), ProductACL.List)),
                    new ChildMenuItem("Categories", "/Admin/Apps/Ecommerce/Category",
                        ACLOption.Create(new CategoryACL(), CategoryACL.List)),
                    new ChildMenuItem("Brands", "/Admin/Apps/Ecommerce/Brand",
                        ACLOption.Create(new BrandACL(), BrandACL.List)),
                    new ChildMenuItem("Product Specifications", "/Admin/Apps/Ecommerce/ProductSpecificationAttribute",
                        ACLOption.Create(new ProductSpecificationAttributeACL(), ProductSpecificationAttributeACL.List)),
                    new ChildMenuItem("Discounts", "/Admin/Apps/Ecommerce/Discount",
                        ACLOption.Create(new DiscountACL(), DiscountACL.List)),
                };
                if (_ecommerceSettings.GiftCardsEnabled)
                {
                    adminItems.Add(new ChildMenuItem("Gift Cards", "/Admin/Apps/Ecommerce/GiftCard",
                        ACLOption.Create(new GiftCardACL(), GiftCardACL.List)));
                }

                var ecommerceMenu = new SubMenu();
                ecommerceMenu.AddRange(adminItems);
                ecommerceMenu.Add(new ChildMenuItem("Templates", "/Admin/Apps/Ecommerce/NewsletterTemplate",
                    ACLOption.Create(new NewsletterTemplateACL(), NewsletterTemplateACL.List)));
                ecommerceMenu.Add(new ChildMenuItem("Newsletter", "/Admin/Apps/Ecommerce/Newsletter",
                    ACLOption.Create(new NewsletterACL(), NewsletterACL.List)));
                return ecommerceMenu;
            }
        }

        public int DisplayOrder
        {
            get { return 45; }
        }
    }
}
