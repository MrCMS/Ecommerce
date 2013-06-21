$(function () {
    $(document).ready(function () {
        search();
    });
    
    $(document).on('change', "select[id='Country_Id']", function () {
        search();
    });
    
    function search() {
        $.get('/Apps/Ecommerce/Cart/ShippingMethods',
            {
                id: $("#Country_Id").val()
            }, function (shippingMethods) {
                $('#shippingMethods').html(shippingMethods);
            });
    }
    
    $(document).on('click', "#continue", function () {
        if ($("#ShippingMethods").val() == null) {
            search();
        } else {
            $("form#deliveryDetails").submit();
        }
    });
});