$(function () {
    $("input[name='havePassword']").change(function() {
        setPasswordBox(this);
    });

    function setPasswordBox(control) {
        if ($(control).val() === "true") {
            $('#password-box').show();
            return;
        } else {
            $('#password-box').hide();
        }
    }
})

