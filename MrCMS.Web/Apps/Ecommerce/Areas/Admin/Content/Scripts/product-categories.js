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
    $(document).on('click', '#categories .pagination a', function () {
        var href = $(this).attr('href');
        if (href != null && href != '') {
            $('.modal-body').load(href + ' div#categories', function () {
                resizeModal();
            });
        }
        return false;
    });
    $(document).on('click', '#categories .add-category', function () {
        var button = $(this),
            productId = button.data('product-id'),
            categoryId = button.data('category-id'),
            page = button.data('page');
        $.post('/Admin/Apps/Ecommerce/Product/AddCategory/',
            { id: productId, categoryId: categoryId },
            function (response) {
                parent.$.get('/Admin/Apps/Ecommerce/Product/ViewCategories/' + productId, function (products) {
                    parent.$('#category-list').replaceWith(products);
                });
                var href = '/Admin/Apps/Ecommerce/Product/AddCategory/' + productId + '?page=' + page;
                $('.modal-body').load(href + ' div#categories', function () {
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
    }
})