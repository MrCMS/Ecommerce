$(function () {
    $('button#close').click(function () {
        parent.$.fancybox.close();
        return false;
    });

    $(document).on('click', '#specifications .add-specification', function () {
        var button = $(this),
            specificationId = button.data('specification-id'),
            categoryId = $('#categoryId').val();
        $.post('/Admin/Apps/Ecommerce/Category/AddSpecification/',
            { id: specificationId, categoryId: categoryId },
            function (response) {
                parent.$.get('/Admin/Apps/Ecommerce/Category/HiddenSpecifications/' + categoryId, function (products) {
                    parent.$('#hidden-specifications').replaceWith(products);
                });
                var href = '/Admin/Apps/Ecommerce/Category/AddSpecification/' + categoryId;
                $('.modal-body-container').load(href + ' div#specifications', function() {
                });
            });
        return false;
    });
})

