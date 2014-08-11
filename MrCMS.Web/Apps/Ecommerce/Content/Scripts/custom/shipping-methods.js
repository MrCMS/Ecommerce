$(function () {
    function setToEditMethod() {
        $.get('/Apps/Ecommerce/SetShippingAddress/ShowShippingAddress', function (result) {
            $('#address-info').html(result);
        });
        $.get('/Apps/Ecommerce/SetShippingMethod/ShippingOptions', function (result) {
            $('#method-info').html(result);
            showHideToPayment();
        });
    }
    function setToEditAddress() {
        $.get('/Apps/Ecommerce/SetShippingAddress/ShippingAddress', function (result) {
            $('#address-info').html(result);
        });
        $.get('/Apps/Ecommerce/SetShippingMethod/AwaitingAddress', function (result) {
            $('#method-info').html(result);
        });
    }
    function updateSummary() {
        $.get('/Apps/Ecommerce/Checkout/Summary', function (result) {
            $('#basic-details').replaceWith(result);
        });

    }
    function showHideToPayment() {
        var value = $("select#ShippingMethod").val();
        if (value) {
            $("#to-payment-details").show();
        } else {
            $("#to-payment-details").hide();
        }
    }
    showHideToPayment();
    $(document).on('change', "select#ShippingMethod", function () {
        $.post('/Apps/Ecommerce/SetShippingMethod/SetShipping',
        {
            method: $(this).val()
        }, function () {
            updateSummary();
            showHideToPayment();
        });
    });
    $(document).on('click', '[data-edit-address]', function (event) {
        event.preventDefault();
        setToEditAddress();
    });
    $(document).on('#shipping-address').submit(function (event) {
        event.preventDefault();
        var form = $(event.target);
        $.post(form.attr('action'), form.serialize(), function (data) {
            if (data) {
                setToEditMethod();
            }
        });
    });
    $(document).on('change', "select#existing-addresses", function () {
        var address = $.parseJSON($(this).val());
        if (address != null) {
            for (var key in address) {
                if ($('#delivery-address-container #' + key).length) {
                    $('#delivery-address-container #' + key).val(address[key]);
                }
            }
        }
    });
});