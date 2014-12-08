(function ($) {
    function setPaymentMethod(event) {
        var checked = $('input[name="PaymentMethodSystemName"]:checked').val();
        if (checked) {
            $.post('/Apps/Ecommerce/PaymentDetails/SetPaymentMethod', { paymentMethod: checked }, function (url) {
                $.get(url, function (response) {
                    $('#payment-confirmation').html(response);
                    $.validator.unobtrusive.parse('form');
                });
            });
        }
    }
    function submitSetAsShippingForm(event) {
        var form = $(event.target).parents('form');
        form.submit();
    }
    function setAddress(event) {
        var address = $.parseJSON($(this).val());
        if (address != null) {
            for (var key in address) {
                if ($('#billing-address #' + key).length) {
                    $('#billing-address #' + key).val(address[key]);
                }
            }
        }
    }

    $(function () {
        $(document).on('change', '#SameAsShipping', submitSetAsShippingForm);
        $(document).on('change', "select#existing-addresses", setAddress);
        $(document).on('change', 'input[name="PaymentMethodSystemName"]', setPaymentMethod);
        $(document).on('add-discount', setPaymentMethod);
        $(document).on('remove-discount', setPaymentMethod);

        setPaymentMethod();
    });
})(jQuery);
