$(function () {
    $("input[name='havePassword']").change(function() {
        if ($(this).val() === "true") {
            $('#password-box').show();
            return;
        } else {
            $('#password-box').hide();
        }
    });
})

