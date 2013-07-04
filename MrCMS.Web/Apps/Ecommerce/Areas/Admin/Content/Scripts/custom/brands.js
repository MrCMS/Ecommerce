$(document).ready(function () {
    $('#searchParam').keypress(function(key) {
        var searchParam = $(this).val();
        if (e.which == 13 && searchParam!="") {
                $('form').submit();
        }
    });
    $(document).on('click', 'button#close', function () {
        var button = $(this);
        var name = $('#Name').val();
        if (name !== "") {
            $.post('/Admin/Apps/Ecommerce/Product/AddBrand',
                { name: name },
                function (response) {
                    if (response === false) {
                        alert("Please try again with different name.");
                    }
                    else {
                        parent.$.get('/Admin/Apps/Ecommerce/Product/Brands', { brandId: response }, function (brands) {
                            parent.$('#brands').replaceWith(brands);
                        });
                        parent.$.fancybox.close();
                    }
                });
        }
        return false;
    });
});