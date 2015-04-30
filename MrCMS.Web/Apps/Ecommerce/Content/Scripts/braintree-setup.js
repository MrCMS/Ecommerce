
(function($) {
    
    $.get("/Apps/Ecommerce/Braintree/GetToken"), function (response) {
        var token = response;
    };
    
})(jQuery);

braintree.setup(
  "@Model.ClientToken",
  'dropin', {
      container: 'dropin'
  });