$(document).ready(function () {
    $('#searchParam').keypress(function (key) {
        var searchParam = $(this).val();
        if (e.which == 13 && searchParam != "") {
            $('form').submit();
        }
    });
    $(document).on('click', 'button#close', function () {
        var name = $('#Name').val();
        var productId = $('#productId').val();
        if (name !== "") {
            $.post('/Admin/Apps/Ecommerce/Product/AddBrand',
                { name: name },
                function (response) {
                    if (response === false) {
                        alert("Please try again with different name.");
                    }
                    else {
                        parent.$.post('/Admin/Webpage/Edit/' + productId, { "Brand.Id": response }, function (resp) {
                            parent.$.get('/Admin/Apps/Ecommerce/Product/Brands', { Id: productId }, function (brands) {
                                parent.$('#brands').html(brands);
                                parent.$.fancybox.close();
                            });
                        });
                    }
                });
        }
        return false;
    });
});