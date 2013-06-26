$(function () {
    $(document).ready(function () {
        $.get('/Apps/Ecommerce/Cart/ShippingMethods',
           {
               id: $("#Country_Id").val()
           }, function (shippingMethods) {
               $('#shippingMethods').html(shippingMethods);
               $("#ShippingMethodValue").change();
               updateShippingMethod();
           });
    });
    
    $(document).on('change', "select[id='Country_Id']", function () {
        search();
        $("#shippingMethods").change();
    });
    
    $(document).on('change', "select[id='ShippingMethods']", function () {
        updateShippingMethod();
    });
    
    $(document).on('change', "#shippingMethods", function () {
        parent.$.get('/Apps/Ecommerce/Cart/BasicDetails', function (result) {
            parent.$('#basic-details').replaceWith(result);
        });
    });
    
    $(document).on('change', "#ShippingMethodValue", function () {
        var sm = $(this).val();
        if (sm !== "0") {
            $("select[id='ShippingMethods']").val(sm);
        }
    });
    
    function search() {
        $.get('/Apps/Ecommerce/Cart/ShippingMethods',
            {
                id: $("#Country_Id").val()
            }, function (shippingMethods) {
                $('#shippingMethods').html(shippingMethods);
                updateShippingMethod();
            });
    }
    
    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/Cart/AddShippingMethod',
           { id: $("select[id='ShippingMethods']").val() },
           function (response) {
               parent.$.get('/Apps/Ecommerce/Cart/BasicDetails', function (result) {
                   parent.$('#basic-details').replaceWith(result);
               });
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