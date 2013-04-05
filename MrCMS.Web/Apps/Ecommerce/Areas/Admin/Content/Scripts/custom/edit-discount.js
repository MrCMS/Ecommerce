$('#LimitationOpt').change(function () {
    var selectedID = $(this).val();
    var dID = $("#Id").val();
    if (selectedID != "") {
        $.get('/Admin/Apps/Ecommerce/DiscountLimitation/LoadDiscountLimitationProperties?limitationID=' + selectedID + "&discountID=" + dID, function (data) {
            $('#limitationValue').html(data);
            $('#limitationValue').fadeIn('fast');
        });
    }
    else {
        $('#limitationValue').html("");
    }
});
$('#ApplicationOpt').change(function () {
    var selectedID = $(this).val();
    var dID = $("#Id").val();
    if (selectedID != "") {
        $.get('/Admin/Apps/Ecommerce/DiscountApplication/LoadDiscountApplicationProperties?applicationID=' + selectedID + "&discountID=" + dID, function (data) {
            $('#applicationValue').html(data);
            $('#applicationValue').fadeIn('fast');
        });
    }
    else {
        $('#applicationValue').html("");
    }
});

$(document).ready(function () {
    $('#LimitationOpt').trigger('change');
    $('#ApplicationOpt').trigger('change');
});