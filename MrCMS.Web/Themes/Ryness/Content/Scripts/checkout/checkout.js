$(document).ready(function () {
    $('.carousel').carousel();

    $('#rdoYes').attr('value', true).change(
        function () {
            if ($(this).is(':checked')) {
                $("#passwordbox").removeClass();
                $("#passwordbox").addClass("span10");
            }
        });

    $('#rdoNo').change(
    function () {
        if ($(this).is(':checked')) {
            $("#passwordbox").addClass("span11 hidden");
        }
    });
});