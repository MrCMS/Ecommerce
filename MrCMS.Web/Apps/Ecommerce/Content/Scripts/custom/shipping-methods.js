$(function () {
    function reinitializeValidation() {
        var form = $('form');
        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        form.find('input, select').each(function () {
            $.data(this, "previousValue", null);
        });
        $.validator.unobtrusive.parse("form");
    }
    function setHtml(selector, content) {
        $(selector).html(content);
        reinitializeValidation();
    }
    function setToEditMethod() {
        $.get('/Apps/Ecommerce/SetShippingAddress/ShowShippingAddress', function (result) {
            setHtml('#address-info', result);
        });
        $.get('/Apps/Ecommerce/SetShippingMethod/ShippingOptions', function (result) {
            setHtml('#method-info', result);
        });
    }
    function setToEditAddress() {
        $.get('/Apps/Ecommerce/SetShippingAddress/ShippingAddress', function (result) {
            setHtml('#address-info', result);
        });
        $.get('/Apps/Ecommerce/SetShippingMethod/AwaitingAddress', function (result) {
            setHtml('#method-info', result);
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
    showHideToPayment();
});