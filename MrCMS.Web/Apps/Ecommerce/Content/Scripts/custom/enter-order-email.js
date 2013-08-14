$(function () {
    $("input[name='HavePassword']").change(function () {
        setPasswordBox(this);
    });

    function setPasswordBox(control) {
        if ($(control).val() === "True") {
            $('#password-box').show();
        } else {
            $('#password-box').hide();
        }
        return false;
    }
})

