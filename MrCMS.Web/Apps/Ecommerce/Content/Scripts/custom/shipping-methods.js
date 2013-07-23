$(function () {
    $(document).on('change', "select#ShippingCalculation", function () {
        $.post('/Apps/Ecommerce/SetDeliveryDetails/SetShipping',
          { id: $(this).val() }, function () {
              $.get('/Apps/Ecommerce/Checkout/Summary', function (result) {
                  $('#basic-details').replaceWith(result);
              });
              $.get("Apps/Ecommerce/SetDeliveryDetails/DeliveryAddress", function (response) {
                  $('#delivery-address-container').replaceWith(response);
                  $.validator.unobtrusive.parse('form');
              });
          });
    });
});