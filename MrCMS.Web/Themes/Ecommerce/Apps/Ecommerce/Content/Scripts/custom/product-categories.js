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
                parent.$.get('/Admin/Apps/Ecommerce/Product/Categories/' + productId, function (products) {
                    parent.$('#category-list').replaceWith(products);
                });
                var href = '/Admin/Apps/Ecommerce/Product/AddCategory/' + productId + '?page=' + page;
                $('.modal-body-container').load(href + ' div#categories', function () {
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
        if (term !== "") {
            $.getJSON('/Admin/Apps/Ecommerce/Product/SearchCategories',
            { Id: productId, term: term },
            function (response) {
                $("table").empty();
                $.each(response, function(key,val) {
                    $("table").append("<tr><td>" + val["Name"] + "</td><td><div class=\"pull-right\"><button data-page=\"0\" data-product-id=\"" + productId + "\"data-category-id=" + val["CategoryID"] + " class=\"btn btn-success add-category\">Add</button></div></td></tr>");
                })
            });
        }
    });
})

