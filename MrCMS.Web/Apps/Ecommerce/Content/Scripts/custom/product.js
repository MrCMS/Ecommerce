$(function () {
    $('#variant').change(function() {
        location.href = location.pathname + "?variant=" + $('#variant').val();
    });
})

