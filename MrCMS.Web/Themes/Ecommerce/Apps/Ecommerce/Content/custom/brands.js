$(document).ready(function () {
    $('#searchParam').keypress(function(key) {
        var searchParam = $(this).val();
        if (e.which == 13 && searchParam!="") {
                $('form').submit();
        }
    });
});