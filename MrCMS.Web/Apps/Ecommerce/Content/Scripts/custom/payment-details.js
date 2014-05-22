$(function () {
    $('#SameAsShipping').change(function () {
        var parents = $(this).parents('form');
        parents.submit();
    });
    $(document).on('change', "select#existing-addresses", function () {
        var address = $.parseJSON($(this).val());
        if (address != null) {
            for (var key in address) {
                if ($('#billing-address #' + key).length) {
                    $('#billing-address #' + key).val(address[key]);
                }
            }
        }
    });
    $(document).on('change', 'input[name="PaymentMethodSystemName"]', function () {
        var checked = $('input[name="PaymentMethodSystemName"]:checked').val();
        $.post('/Apps/Ecommerce/PaymentDetails/SetPaymentMethod', { paymentMethod: checked }, function (url) {
            $.get(url, function (response) {
                $('#payment-confirmation').html(response);
                $.validator.unobtrusive.parse('form');
            });
        });
    });
})

