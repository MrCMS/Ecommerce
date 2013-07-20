$(function () {
    $('#useDeliveryAddress').change(function () {
        if ($(this).attr("checked")) {
            $('#billingAddress').hide();
            $('#deliveryAddress').show();
            return;
        }
        else
        {
            $('#deliveryAddress').hide();
            $('#billingAddress').show();
        }
    });
})

