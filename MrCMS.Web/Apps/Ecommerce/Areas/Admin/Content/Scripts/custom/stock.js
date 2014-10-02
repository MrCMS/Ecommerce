$(function () {
    //UPDATE PRODUCT VARIANT STOCK
    $(document).on('click', 'button.update-stock', function () {
        var form= $(this).parents('form');
        $.post(form.attr('action'),
                form.serialize(),
            function (response) {
                if (response) {
                    $('#product-variants').load(window.location.href + ' #product-variants');
                } else {
                    alert("Error happened during update operation. Please check your parameters and try again.");
                }
            });
        return false;
    });
})