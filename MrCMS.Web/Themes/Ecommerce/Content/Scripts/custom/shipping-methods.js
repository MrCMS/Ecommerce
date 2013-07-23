$(function () {
    $(document).on('change', "select[id='ShippingCalculation']", function () {
        updateShippingMethod();
    });
    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/Cart/AddShippingMethod',
           { id: $("select[id='ShippingCalculation']").val() },
           function (response) {
               $.get('/Apps/Ecommerce/Cart/BasicDetails', function (result) {
                   $('#basic-details').replaceWith(result);
               });
               $.get("Apps/Ecommerce/SetDeliveryDetails/DeliveryAddress", function (response) {
                   $('#delivery-address-container').replaceWith(response);
                   $.validator.unobtrusive.parse('form');
               });
           });

    }
});