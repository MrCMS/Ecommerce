using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Web.Apps.Ecommerce.Controllers;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce
{
    public static class EcommerceRouteConfig
    {
        public static void RegisterRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Admin controllers", "Admin", "Admin/Apps/Ecommerce/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional }, new[] { typeof(ProductController).Namespace });
            context.MapRoute("Product Variant - GetPriceBreaksForProductVariant", "Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant", new { controller = "ProductVariant", action = "GetPriceBreaksForProductVariant" });

            MapCartRoutes(context);
            MapCheckoutRoutes(context);
            MapPaymentMethodRoutes(context);
            MapRemoteValidationRoutes(context);

            context.MapRoute("User Account Orders", "Apps/Ecommerce/UserAccount/UserAccountOrders", new { controller = "UserAccount", action = "UserAccountOrders" });
            context.MapRoute("User Account - download Order PDF", "Apps/Ecommerce/OrderPdf/ExportOrderToPdf/{id}", new { controller = "OrderPdf", action = "ExportOrderToPdf" }, new[] { typeof(OrderPdfController).Namespace });
            context.MapRoute("Product Search - Query", "search/query", new { controller = "ProductSearch", action = "Query" }, new[] { typeof(ProductSearchController).Namespace });
            context.MapRoute("Product Search - Results", "search/results", new { controller = "ProductSearch", action = "Results" }, new[] { typeof(ProductSearchController).Namespace });
            context.MapRoute("Category Container - Categories", "Apps/Ecommerce/CategoryContainer/Categories", new { controller = "CategoryContainer", action = "Categories" }, new[] { typeof(CategoryContainer).Namespace });

            context.MapRoute("Export Google Base Feed", "export/google-base-feed", new { controller = "GoogleBaseFeed", action = "ExportProductsToGoogleBaseInResponse" }, new[] { typeof(GoogleBaseFeedController).Namespace });

            context.MapRoute("Products - Back in stock request", "Apps/Ecommerce/Product/BackInStock", new { controller = "Product", action = "BackInStock" }, new[] { typeof(ProductController).Namespace });


            context.MapRoute("Download Product", "digital-download/{guid}/{id}", new { controller = "DownloadOrderedFile", action = "Download" }, new[] { typeof(DownloadOrderedFileController).Namespace });

            context.MapRoute("Product - Get Variant Details", "product/variant-details/{id}", new { controller = "Product", action = "VariantDetails" }, new[] { typeof(ProductController).Namespace });
        }

        private static void MapRemoteValidationRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Can add quantity to cart", "can-add-quantity", new {controller = "Cart", action = "CanAddQuantity"}, new[] {typeof (CartController).Namespace});
        }

        private static void MapPaymentMethodRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("PayPal Express Checkout - SetExpressCheckout", "Apps/Ecommerce/PayPalExpress/SetExpressCheckout", new { controller = "PayPalExpressCheckout", action = "SetExpressCheckout" },
                            new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("PayPal Express Checkout - IPN", "Apps/Ecommerce/PayPalExpress/IPN", new { controller = "PayPalExpressCheckout", action = "IPN" },
                             new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("Checkout - PayPal Return Handler", "Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler", new { controller = "PayPalExpressCheckout", action = "Return" }, new[] { typeof(PayPalExpressCheckoutController).Namespace });

            context.MapRoute("Checkout - Paypoint 3D Secure Redirect", "Apps/Ecommerce/Paypoint/3DSecureRedirect", new { controller = "Paypoint", action = "Redirect3DSecure" }, new[] { typeof(PaypointController).Namespace });
            context.MapRoute("Checkout - Paypoint 3D Secure Response Handler", "Apps/Ecommerce/Paypoint/3DSecureReturnHandler", new { controller = "Paypoint", action = "Response3DSecure" }, new[] { typeof(PaypointController).Namespace });

            context.MapRoute("Checkout - SagePay Notification Url", "Apps/Ecommerce/SagePay/Notification", new { controller = "SagePayNotification", action = "Notification" }, new[] { typeof(SagePayNotificationController).Namespace });
            context.MapRoute("Checkout - SagePay Success Url", "Apps/Ecommerce/SagePay/Success/{vendorTxCode}", new { controller = "SagePayRedirect", action = "Success" }, new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Success POST", "Apps/Ecommerce/SagePay/SuccessRedirect", new { controller = "SagePayRedirect", action = "SuccessPost" }, new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Failed Url", "Apps/Ecommerce/SagePay/Failed/{vendorTxCode}", new { controller = "SagePayRedirect", action = "Failed" }, new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Failed POST", "Apps/Ecommerce/SagePay/FailedRedirect", new { controller = "SagePayRedirect", action = "FailedPost" }, new[] { typeof(SagePayRedirectController).Namespace });

        }


        private static void MapCheckoutRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Set Delivery Details - Get Delivery Address", "Apps/Ecommerce/SetDeliveryDetails/DeliveryAddress", new { controller = "SetDeliveryDetails", action = "DeliveryAddress" },
                             new[] { typeof(SetDeliveryDetailsController).Namespace });
            context.MapRoute("Set Delivery Details - Set Shipping Method", "Apps/Ecommerce/SetDeliveryDetails/SetShipping", new { controller = "SetDeliveryDetails", action = "SetShipping" },
                             new[] { typeof(SetDeliveryDetailsController).Namespace });
            context.MapRoute("Set Delivery Details - Set Shipping Address", "Apps/Ecommerce/SetDeliveryDetails/SetAddress", new { controller = "SetDeliveryDetails", action = "SetAddress" },
                             new[] { typeof(SetDeliveryDetailsController).Namespace });
            context.MapRoute("Enter Order Email - Set Order Email", "Apps/Ecommerce/SetOrderEmail", new { controller = "EnterOrderEmail", action = "SetOrderEmail" },
                             new[] { typeof(EnterOrderEmailController).Namespace });
            context.MapRoute("Enter Order Email - Set Order Email And Login", "Apps/Ecommerce/SetOrderEmailAndLogin", new { controller = "EnterOrderEmail", action = "SetOrderEmailAndLogin" },
                             new[] { typeof(EnterOrderEmailController).Namespace });
            context.MapRoute("Checkout - Price Summary", "Apps/Ecommerce/Checkout/Summary", new { controller = "Checkout", action = "Summary" },
                             new[] { typeof(CheckoutController).Namespace });
            context.MapRoute("Confirm Order - Cash On Delivery", "Apps/Ecommerce/Confirm/CashOnDelivery", new { controller = "PaymentMethod", action = "CashOnDelivery" },
                             new[] { typeof(PaymentMethodController).Namespace });
            context.MapRoute("Confirm Order - PayPal Express Checkout", "Apps/Ecommerce/Confirm/PayPalExpressCheckout", new { controller = "PaymentMethod", action = "PayPalExpressCheckout" },
                             new[] { typeof(PaymentMethodController).Namespace });
            context.MapRoute("Confirm Order - Paypoint", "Apps/Ecommerce/Confirm/Paypoint", new { controller = "PaymentMethod", action = "Paypoint" },
                             new[] { typeof(PaymentMethodController).Namespace });
            context.MapRoute("Confirm Order - SagePay", "Apps/Ecommerce/Confirm/SagePay", new { controller = "PaymentMethod", action = "SagePay" },
                             new[] { typeof(PaymentMethodController).Namespace });
            context.MapRoute("Checkout - Billing Address same as Shipping Address", "Apps/Ecommerce/PaymentDetails/BillingAddressSameAsShippingAddress", new { controller = "PaymentDetails", action = "BillingAddressSameAsShippingAddress" },
                             new[] { typeof(PaymentDetailsController).Namespace });
            context.MapRoute("Checkout - Update Billing Address", "Apps/Ecommerce/PaymentDetails/UpdateBillingAddress", new { controller = "PaymentDetails", action = "UpdateBillingAddress" },
                             new[] { typeof(PaymentDetailsController).Namespace });
            context.MapRoute("Checkout - Save Billing Address", "Apps/Ecommerce/PaymentDetails/SaveBillingAddress", new { controller = "PaymentDetails", action = "SaveBillingAddress" }, new[] { typeof(PaymentDetailsController).Namespace });
            context.MapRoute("Checkout - Set Payment Method", "Apps/Ecommerce/PaymentDetails/SetPaymentMethod", new { controller = "PaymentDetails", action = "SetPaymentMethod" }, new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Order Placed - Login and associate order", "order-placed/login", new { controller = "OrderPlaced", action = "LoginAndAssociateOrder" }, new[] { typeof(OrderPlacedController).Namespace });

            context.MapRoute("Order Placed - Register and associate order", "order-placed/register", new { controller = "UserAccount", action = "RegistrationWithoutDetails" }, new[] { typeof(OrderPlacedController).Namespace });
        }

        private static void MapCartRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Cart - Details", "Apps/Ecommerce/CartDetails", new { controller = "Cart", action = "Details" });
            context.MapRoute("Cart - Update Basket", "Apps/Ecommerce/UpdateBasket", new { controller = "Cart", action = "UpdateBasket" });
            context.MapRoute("Cart - Empty Basket", "Apps/Ecommerce/EmptyBasket", new { controller = "Cart", action = "EmptyBasket" });
            context.MapRoute("Cart - Add to Cart", "Apps/Ecommerce/AddToCart", new { controller = "Cart", action = "AddToCart" });
            context.MapRoute("Cart - Edit Cart Item", "Apps/Ecommerce/EditCartItem", new { controller = "Cart", action = "EditCartItem" });
            context.MapRoute("Cart - Delete Cart Item", "Apps/Ecommerce/DeleteCartItem", new { controller = "Cart", action = "DeleteCartItem" });
            context.MapRoute("Cart - Apply Discount Code", "Apps/Ecommerce/ApplyDiscountCode", new { controller = "Cart", action = "ApplyDiscountCode" });
        }
    }
}