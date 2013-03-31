$('#LimitationOpt').change(function () {
    var selectedID = $(this).val();
    var dID = $("#Id").val();
    if (selectedID != "") {
        $.get('/Admin/Apps/Ecommerce/Discount/LoadDiscountLimitationProperties?lid=' + selectedID + "&did=" + dID, function (data) {
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
        $.get('/Admin/Apps/Ecommerce/Discount/LoadDiscountApplicationProperties?aid=' + selectedID + "&did=" + dID, function (data) {
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

$('.modal-body').css("max-height", "1500px");
$('.modal-body').css("max-height", "1500px");