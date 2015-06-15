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
            context.MapAreaRoute("Admin controllers", "Admin", "Admin/Apps/Ecommerce/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(ProductController).Namespace });
            context.MapRoute("Product Variant - GetPriceBreaksForProductVariant",
                "Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant",
                new { controller = "ProductVariant", action = "GetPriceBreaksForProductVariant" });

            MapCartRoutes(context);
            MapCheckoutRoutes(context);
            MapPaymentMethodRoutes(context);
            MapRemoteValidationRoutes(context);
            MapWishlistRoutes(context);
            MapUserAccount(context);

            context.MapRoute("User Account Orders", "Apps/Ecommerce/UserAccount/UserAccountOrders",
                new { controller = "UserAccount", action = "UserAccountOrders" });
            context.MapRoute("User Account - download Order PDF", "Apps/Ecommerce/OrderPdf/ExportOrderToPdf/{id}",
                new { controller = "OrderPdf", action = "ExportOrderToPdf" },
                new[] { typeof(OrderPdfController).Namespace });
            context.MapRoute("Product Search - Query", "search/query",
                new { controller = "ProductSearch", action = "Query" }, new[] { typeof(ProductSearchController).Namespace });
            context.MapRoute("Product Search - Results", "search/results",
                new { controller = "ProductSearch", action = "Results" },
                new[] { typeof(ProductSearchController).Namespace });

            context.MapRoute("Brand Search - Query", "brand/search/query",
                new { controller = "Brand", action = "Query" }, new[] { typeof(BrandController).Namespace });
            context.MapRoute("Brand Search - Results", "brand/search/results",
                new { controller = "Brand", action = "Results" },
                new[] { typeof(BrandController).Namespace });

            context.MapRoute("Category Container - Categories", "Apps/Ecommerce/CategoryContainer/Categories",
                new { controller = "CategoryContainer", action = "Categories" },
                new[] { typeof(ProductContainer).Namespace });

            context.MapRoute("Export Google Base Feed", "export/google-base-feed",
                new { controller = "GoogleBaseFeed", action = "ExportProductsToGoogleBaseInResponse" },
                new[] { typeof(GoogleBaseFeedController).Namespace });

            context.MapRoute("Products - Back in stock request", "Apps/Ecommerce/Product/BackInStock",
                new { controller = "Product", action = "BackInStock" }, new[] { typeof(ProductController).Namespace });


            context.MapRoute("Download Product", "digital-download/{guid}/{id}",
                new { controller = "DownloadOrderedFile", action = "Download" },
                new[] { typeof(DownloadOrderedFileController).Namespace });

            context.MapRoute("Product - Get Variant Details", "product/variant-details/{id}",
                new { controller = "ProductVariant", action = "Details" }, new[] { typeof(ProductVariantController).Namespace });

            //Product Reviews
            context.MapRoute("Product Review", "Apps/Ecommerce/ProductReview/Add",
                new { controller = "ProductReview", action = "Add" },
                new[] { typeof(ProductReviewController).Namespace });

            context.MapRoute("Product Reviews Paging", "Apps/Ecommerce/ProductVariant/ProductReviews",
                new { controller = "ProductVariant", action = "ProductReviews" });

            context.MapRoute("Product Review Helpfulness Vote Yes", "Apps/Ecommerce/ProductReview/HelpfulnessVotes",
                new { controller = "ProductReview", action = "HelpfulnessVotes" },
                new[] { typeof(ProductReviewController).Namespace });

            context.MapRoute("Product Review Helpfulness Vote No", "Apps/Ecommerce/ProductReview/UnhelpfulnessVotes",
                new { controller = "ProductReview", action = "UnhelpfulnessVotes" },
                new[] { typeof(ProductReviewController).Namespace });

            // Public Routes
            context.MapRoute("Generate Contact Us Map", "get-contact-map",
                new { controller = "ContactUs", action = "GenerateMap" },
                new[] { typeof(ContactUsController).Namespace });
        }

        private static void MapUserAccount(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Update User Info", "user-account/handle/user-info",
                new {controller = "UserAccountInfo", action = "UpdateUserInfo"},
                new[] {typeof (UserAccountInfoController).Namespace});

            context.MapRoute("Update Password", "user-account/handle/update-password",
                new { controller = "UserAccountChangePassword", action = "UpdatePassword" },
                new[] {typeof (UserAccountChangePasswordController).Namespace});

            context.MapRoute("User Account - Edit", "user-account/handle/edit-address",
                new { controller = "UserAccountEditAddress", action = "Edit" },
                new[] {typeof (UserAccountEditAddressController).Namespace});

            context.MapRoute("User Account - Delete Address", "user-account/handle/delete-address/{id}",
                new {controller = "UserAccountAddresses", action = "DeleteAddress"},
                new[] {typeof (UserAccountAddressesController).Namespace});
        }

        private static void MapWishlistRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Wishlist - Add to Wishlist", "Apps/Ecommerce/AddToWishlist",
                new { controller = "Wishlist", action = "Add" },
                new[] { typeof(WishlistController).Namespace });
            context.MapRoute("Wishlist - Remove from Wishlist", "Apps/Ecommerce/RemoveFromWishlist",
                new { controller = "Wishlist", action = "Remove" },
                new[] { typeof(WishlistController).Namespace });
            context.MapRoute("Wishlist - Summary", "Apps/Ecommerce/WishlistSummary",
                new { controller = "Wishlist", action = "Summary" },
                new[] { typeof(WishlistController).Namespace });
        }

        private static void MapRemoteValidationRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Can add quantity to cart", "can-add-quantity",
                new { controller = "Cart", action = "CanAddQuantity" }, new[] { typeof(CartController).Namespace });
        }

        private static void MapPaymentMethodRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Cash On Delivery - Form", "Apps/Ecommerce/Confirm/CashOnDelivery",
                new { controller = "CashOnDelivery", action = "Form" },
                new[] { typeof(CashOnDeliveryController).Namespace });

            context.MapRoute("Payment Not Required - Form", "Apps/Ecommerce/Confirm/PaymentNotRequired",
                new { controller = "PaymentNotRequired", action = "Form" },
                new[] { typeof(PaymentNotRequiredController).Namespace });

            context.MapRoute("Paypoint - Form", "Apps/Ecommerce/Confirm/Paypoint",
                new { controller = "Paypoint", action = "Form" },
                new[] { typeof(PaypointController).Namespace });
            context.MapRoute("SagePay - Form", "Apps/Ecommerce/Confirm/SagePay",
                new { controller = "SagePay", action = "Form" },
                new[] { typeof(SagePayController).Namespace });

            context.MapRoute("PayPal Express Checkout - Form", "Apps/Ecommerce/Confirm/PaypalExpressCheckout",
                new { controller = "PayPalExpressCheckout", action = "Form" },
                new[] { typeof(PayPalExpressCheckoutController).Namespace });


            context.MapRoute("PayPal Express Checkout - SetExpressCheckout",
                "Apps/Ecommerce/PayPalExpress/SetExpressCheckout",
                new { controller = "PayPalExpressCheckout", action = "SetExpressCheckout" },
                new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("PayPal Express Checkout - Reset",
                "Apps/Ecommerce/PayPalExpress/Reset",
                new { controller = "PayPalExpressCheckout", action = "Reset" },
                new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("PayPal Express Checkout - IPN", "Apps/Ecommerce/PayPalExpress/IPN",
                new { controller = "PayPalExpressIPN", action = "Handle" },
                new[] { typeof(PayPalExpressIPNController).Namespace });
            context.MapRoute("Checkout - PayPal Return Handler", "Apps/Ecommerce/PayPalExpressCheckout/ReturnHandler",
                new { controller = "PayPalExpressCheckout", action = "Return" },
                new[] { typeof(PayPalExpressCheckoutController).Namespace });
            context.MapRoute("Checkout - PayPal Callback Handler",
                "Apps/Ecommerce/PayPalExpressCheckout/CallbackHandler",
                new { controller = "PayPalExpressCallback", action = "Handler" },
                new[] { typeof(PayPalExpressCallbackController).Namespace });

            context.MapRoute("Checkout - Paypoint 3D Secure Redirect", "Apps/Ecommerce/Paypoint/3DSecureRedirect",
                new { controller = "Paypoint", action = "Redirect3DSecure" },
                new[] { typeof(PaypointController).Namespace });
            context.MapRoute("Checkout - Paypoint 3D Secure Response Handler",
                "Apps/Ecommerce/Paypoint/3DSecureReturnHandler",
                new { controller = "Paypoint", action = "Response3DSecure" },
                new[] { typeof(PaypointController).Namespace });

            context.MapRoute("Checkout - SagePay Notification Url", "Apps/Ecommerce/SagePay/Notification",
                new { controller = "SagePayNotification", action = "Notification" },
                new[] { typeof(SagePayNotificationController).Namespace });
            context.MapRoute("Checkout - SagePay Success Url", "Apps/Ecommerce/SagePay/Success/{vendorTxCode}",
                new { controller = "SagePayRedirect", action = "Success" },
                new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Success POST", "Apps/Ecommerce/SagePay/SuccessRedirect",
                new { controller = "SagePayRedirect", action = "SuccessPost" },
                new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Failed Url", "Apps/Ecommerce/SagePay/Failed/{vendorTxCode}",
                new { controller = "SagePayRedirect", action = "Failed" },
                new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("Checkout - SagePay Failed POST", "Apps/Ecommerce/SagePay/FailedRedirect",
                new { controller = "SagePayRedirect", action = "FailedPost" },
                new[] { typeof(SagePayRedirectController).Namespace });
            context.MapRoute("WorldPay - Form", "Apps/Ecommerce/Confirm/WorldPay",
                new { controller = "WorldPay", action = "Form" },
                new[] { typeof(WorldPayController).Namespace });
            context.MapRoute("Checkout - WorldPay Notification Url", "Apps/Ecommerce/WorldPay/Notification",
                new { controller = "WorldPay", action = "Notification" },
                new[] { typeof(WorldPayController).Namespace });



            //Charity Clear Routes
            context.MapRoute("CharityClear Form", "Apps/Ecommerce/Confirm/CharityClear",
                new { controller = "CharityClear", action = "Form" },
                new[] { typeof(CharityClearController).Namespace });
            context.MapRoute("CharityClear PaymentInfo Notification",
                 "Apps/Ecommerce/CharityClear/Notification",
                 new { controller = "CharityClear", action = "Notification" },
                 new[] { typeof(CharityClearController).Namespace });

            context.MapRoute("Braintree Form", "Apps/Ecommerce/Confirm/Braintree",
                new {controller = "Braintree", action = "Form"},
                new[] {typeof (BraintreeController).Namespace});

            context.MapRoute("Braintree Form Post", "Apps/Ecommerce/Confirm/BraintreePayment/Card",
                new {controller = "Braintree", action = "MakePaymentCard"},
                new[] {typeof (BraintreeController).Namespace});

            context.MapRoute("Braintree Paypal Post", "Apps/Ecommerce/Confirm/BraintreePayment/Paypal",
                new {controller = "Braintree", action = "MakePaymentPaypal"},
                new[] {typeof (BraintreeController).Namespace});
        }


        private static void MapCheckoutRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Set Shipping Details - Edit Shipping Address",
                "Apps/Ecommerce/SetShippingAddress/ShippingAddress",
                new { controller = "SetShippingAddress", action = "ShippingAddress" },
                new[] { typeof(SetShippingAddressController).Namespace });

            context.MapRoute("Set Shipping Details - Show Shipping Address",
                "Apps/Ecommerce/SetShippingAddress/ShowShippingAddress",
                new { controller = "SetShippingAddress", action = "ShowShippingAddress" },
                new[] { typeof(SetShippingAddressController).Namespace });

            context.MapRoute("Set Shipping Details - Set Shipping Address",
                "Apps/Ecommerce/SetShippingAddress/SetAddress",
                new { controller = "SetShippingAddress", action = "SetAddress" },
                new[] { typeof(SetShippingAddressController).Namespace });


            context.MapRoute("Set Shipping Details - Awaiting Address",
                "Apps/Ecommerce/SetShippingMethod/AwaitingAddress",
                new { controller = "SetShippingMethod", action = "AwaitingAddress" },
                new[] { typeof(SetShippingMethodController).Namespace });

            context.MapRoute("Set Shipping Details - Get Shipping Options",
                "Apps/Ecommerce/SetShippingMethod/ShippingOptions",
                new { controller = "SetShippingMethod", action = "ShippingOptions" },
                new[] { typeof(SetShippingMethodController).Namespace });

            context.MapRoute("Set Shipping Details - Set Shipping Method",
                "Apps/Ecommerce/SetShippingMethod/SetShipping",
                new { controller = "SetShippingMethod", action = "SetShipping" },
                new[] { typeof(SetShippingMethodController).Namespace });

            context.MapRoute("Set Shipping Details - Set Shipping Date",
                "Apps/Ecommerce/SetShippingDate",
                new { controller = "SetShippingDate", action = "SetDate" },
                new[] { typeof(SetShippingDateController).Namespace });


            context.MapRoute("Enter Order Email - Set Order Email", "Apps/Ecommerce/SetOrderEmail",
                new { controller = "EnterOrderEmail", action = "SetOrderEmail" },
                new[] { typeof(EnterOrderEmailController).Namespace });

            context.MapRoute("Enter Order Email - Set Order Email And Login", "Apps/Ecommerce/SetOrderEmailAndLogin",
                new { controller = "EnterOrderEmail", action = "SetOrderEmailAndLogin" },
                new[] { typeof(EnterOrderEmailController).Namespace });

            context.MapRoute("Checkout - Price Summary", "Apps/Ecommerce/Checkout/Summary",
                new { controller = "Checkout", action = "Summary" },
                new[] { typeof(CheckoutController).Namespace });

            context.MapRoute("Checkout - Billing Address same as Shipping Address",
                "Apps/Ecommerce/PaymentDetails/BillingAddressSameAsShippingAddress",
                new { controller = "PaymentDetails", action = "BillingAddressSameAsShippingAddress" },
                new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Update Billing Address", "Apps/Ecommerce/PaymentDetails/UpdateBillingAddress",
                new { controller = "PaymentDetails", action = "UpdateBillingAddress" },
                new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Save Billing Address", "Apps/Ecommerce/PaymentDetails/SaveBillingAddress",
                new { controller = "PaymentDetails", action = "SaveBillingAddress" },
                new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Accept Terms And Conditions", "Apps/Ecommerce/terms-and-conditions/set",
                new { controller = "TermsAndConditions", action = "Set" },
                new[] { typeof(TermsAndConditionsController).Namespace });

            context.MapRoute("Checkout - Set Payment Method", "Apps/Ecommerce/PaymentDetails/SetPaymentMethod",
                new { controller = "PaymentDetails", action = "SetPaymentMethod" },
                new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Checkout - Get Payment Methods", "Apps/Ecommerce/PaymentDetails/Methods",
                new { controller = "PaymentDetails", action = "Methods" },
                new[] { typeof(PaymentDetailsController).Namespace });

            context.MapRoute("Order Placed - Login and associate order", "order-placed/login",
                new { controller = "OrderPlaced", action = "LoginAndAssociateOrder" },
                new[] { typeof(OrderPlacedController).Namespace });

            context.MapRoute("Order Placed - Register and associate order - other", "order-placed/register",
                new { controller = "OrderPlaced", action = "RegisterAndAssociateOrder" },
                new[] { typeof(OrderPlacedController).Namespace });
        }

        private static void MapCartRoutes(MrCMSAppRegistrationContext context)
        {
            context.MapRoute("Cart - Details", "Apps/Ecommerce/CartDetails",
                new { controller = "Cart", action = "Details" },
                new[] { typeof(CartController).Namespace });

            context.MapRoute("Cart - Header Summary", "Apps/Ecommerce/CartHeader",
                new { controller = "Cart", action = "HeaderSummary" },
                new[] { typeof(CartController).Namespace });

            context.MapRoute("Cart - Update Basket", "Apps/Ecommerce/UpdateBasket",
                new { controller = "Cart", action = "UpdateBasket" },
                new[] { typeof(CartController).Namespace });

            context.MapRoute("Cart - Empty Basket", "Apps/Ecommerce/EmptyBasket",
                new { controller = "EmptyBasket", action = "Empty" },
                new[] { typeof(EmptyBasketController).Namespace });

            context.MapRoute("Cart - Add to Cart", "Apps/Ecommerce/AddToCart",
                new { controller = "AddToCart", action = "Add" });

            context.MapRoute("Cart - Delete Cart Item", "Apps/Ecommerce/DeleteCartItem",
                new { controller = "Cart", action = "DeleteCartItem" });

            context.MapRoute("Cart - Apply Discount Code", "Apps/Ecommerce/ApplyDiscountCode",
                new { controller = "Discount", action = "Apply" });

            context.MapRoute("Cart - Remove Discount Code", "Apps/Ecommerce/RemoveDiscountCode",
                new { controller = "Discount", action = "Remove" });

            context.MapRoute("Cart - Apply Gift Card Code", "Apps/Ecommerce/ApplyGiftCardCode",
                new { controller = "GiftCard", action = "Apply" },
                new[] { typeof(GiftCardController).Namespace });

            context.MapRoute("Cart - Remove Gift Card Code", "Apps/Ecommerce/RemoveGiftCardCode",
                new { controller = "GiftCard", action = "Remove" },
                new[] { typeof(GiftCardController).Namespace });

            context.MapRoute("Cart - Save Gift Message", "Apps/Ecommerce/SaveGiftMessage",
                new { controller = "GiftMessage", action = "Save" },
                new[] { typeof(GiftMessageController).Namespace });

            context.MapRoute("Cart - Use Reward Points", "Apps/Ecommerce/UseRewardPoints",
                new { controller = "CartRewardPoints", action = "Save" },
                new[] { typeof(CartRewardPointsController).Namespace });
        }
    }
}