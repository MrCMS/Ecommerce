$(function () {
    $('button#close').click(function () {
        parent.$.fancybox.close();
        return false;
    });
    $('form.form-update-form input').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            $('.submit-form-btn').click();
        }
    });

    $(document).on('click', '#products .add-product', function () {
        var term = $(this).val();
        var button = $(this),
            parentProductId = button.data('parent-product-id'),
            productId = button.data('product-id');
        $.post('/Admin/Apps/Ecommerce/Product/AddRelatedProduct/',
            { id: parentProductId, relatedProductId: productId },
            function (response) {
                parent.$.get('/Admin/Apps/Ecommerce/Product/RelatedProducts/' + parentProductId, function (products) {
                    parent.$('#related-products').replaceWith(products);
                });
                var href = '/Admin/Apps/Ecommerce/Product/AddRelatedProductItems/' + parentProductId + '?page=1&query=' + term;
                $('.modal-body-container').load(href + ' div#products', function () {
                    resizeModal();
                });
            });
        return false;
    });
    function resizeModal() {
        setTimeout(function () {
            parent.$('#fancybox-content').animate({ height: document.documentElement.scrollHeight }, function () {
                parent.$.fancybox.center();
            });
        }, 100);
    };

    $('#searchparam').keyup(function (key) {
        var term = $(this).val();
        var productId = $("#productId").val();
        if (term !== "" && productId!="" && productId!=null) {
            var href = '/Admin/Apps/Ecommerce/Product/AddRelatedProductItems/' + productId + '?page=1&query=' + term;
            $('.modal-body-container').load(href + ' div#products', function () {
                resizeModal();
            });
        }
    });
})

