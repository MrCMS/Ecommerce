# Mr CMS Ecommerce

**Mr CMS eCommerce is an app created for Mr CMS**

Mr CMS eCommerce project is currently supplied with the latest version of Mr CMS, so there is no need to download Mr CMS separately.

# Installation

	1. Open the project in Visual Studio and run the application. An installer will install some demo data.
	2. Once installed you should set up some shipping options under Ecommerce Settings -> Shipping -> Configuration.
	3. Featured products on the homepage are not set on installation. Navigate to the homepage and turn inline editing on and add 4 products to the featured products widget.

# Features

	* Unlimited Products
	* Unlimited Categories / Nested Categories
	* Product Specifications
	* Product Attributes (Color, Size etc)
	* Stock management
	* Lucene based filterable search (automatically managed)
	* Message template management for order confirmations etc
	* Price breaks (buy 10 for £1.99 instead of £2.50)
	* Country based shipping
	* Shipping by weight
	* Payments by: PayPal, Braintree, Paypoint, SagePay, WorldPay and CharityClear
	* Product Reviews
	* Reports - Sales by day, Sales by Payment Type, Sales by Shipping Type, Orders by Shipping Type
	* Import/Export products using Excel
	* Low Stock Report
	* Bulk stock update
	* Bulk Shipping update
	* Order Export
	* NopCommerce import from version 2.80 (more to ve supported)
	* Google Base Integration
	* Brand Management / Brand filterable search
	* Discounts - Automatic, Code based, Time based
	* Discount Limitations - Cart Total Greater than X, Item Has SKU, Item is in Category, Cart has at least X items, Cart Subtotal greater than X, Shipping country is, Shipping postcode starts with, Single Use, Single use per customer
	* Discount Application - Buy X get Y free, Free Shipping, Order total fixed amount, X % from items, X% from order
	* Discount Usage - View discount useage
	* E-tags (show tags over product images such as 2-4-1 or similar)
	* Newsletter builder App
	* Stats App
	* Amazon App - sent products to Amazon, import orders from Amazon
	* Digital download support
	* Reward points system

# Screenshots

![Home](https://mrcms.blob.core.windows.net/web/1/ecommerce-screen-grabs/ecommerce11.png)
![Category View](https://mrcms.blob.core.windows.net/web/1/ecommerce-screen-grabs/ecommerce21.png)
![Add to cart View](https://mrcms.blob.core.windows.net/web/1/mobile-friendly-navigation/mobile-nav-31.png)
![Checkout View](https://mrcms.blob.core.windows.net/web/1/mobile-friendly-navigation/ecommerce41.png)
![Admin View](https://mrcms.blob.core.windows.net/web/1/ecommerce-screen-grabs/ecommerce51.png)

# Limitations

Mr CMS Ecommerce does not currently support multiple currencies or multisite functionality. It also does not yet support the American tax system.

# Contribution

If you would like to contribute to Mr CMS ecommerce by supplying any missing features please do so by submitting a pull request.

# Version History

Version 0.1 (January 2014)
------------
	* Initial release

Version 0.1.1 (23 April 2014)
------------
	* Upgrade to Mr CMS 0.4
	* Added health checks for payment gateways
	* Category nodes limited to 100

Version 0.2 
------------
	* Upgrade to Mr CMS 0.4.1
		* Localise resources like brand text etc 
	* Newsletter Builder
	* Allow filtering to be turned on/off per spec/attribute
	* Check over index management set up related updates do not action index update
	* In Order Placed email use product name and variant name for items
	* Create generic design
	* Create discount code limitation to SKU and Category

Version 0.2.1
------------
	* Better discounts
	* 4-2-1
	* BOGOF
	* Product reviews
	* Proper pluggable payment system
	* PDF to be done properly for VAT (tax per line)

Version 0.3
------------
	* Product reviews
	* Brand / Product search (admin)
	* Refactor discounts (2-4-1 etc available)
	* Reward points
	* nopCommerce import from 2.80
	* Gift Cards
	* Gift Message
	* Search log
	* Dashboard graph axis fixed
	* Charity Clear payment gateway
	* Batch importer
	* Warehouse based stock
	* Added an OrderOn Date instead of CreatedOn
	* Add button to stop shipment emails on tools page

Version 0.4 (May 2015)
-----------
	* Custom stock messages for in/out of stock
	* Brand search page and search
	* URL Generator configuration for products/brands/categories	
	* Review score can be viewed on product card
	* E-tag admin
	* My account pages broken down into: details, change password, orders, reviews, rewards, addresses		
	* Reward points can be used during checkout
	* Newsletter build made into app
	* Braintree support

Version 0.5 (Future)
	* Feedback app
	* nopCommerce 1.9 import
