$(document).ready(function () {
    $('#searchParam').keypress(function (key) {
        var searchParam = $(this).val();
        if (e.which == 13 && searchParam != "") {
            $('form').submit();
        }
    });
    $(document).on('submit', '#add-brand-inline', function (event) {
        var form = $(event.target);
        event.preventDefault();
        var productId = form.find('#productId').val();
        if (form.valid()) {
            $.post(form.attr('action'),
                form.serialize(),
                function (response) {
                    if (response === false) {
                        alert("Please try again with different name.");
                    } else {
                        parent.$.post('/Admin/Webpage/Edit/' + productId, { "Brand.Id": response }, function (resp) {
                            parent.$.get('/Admin/Apps/Ecommerce/Product/Brands', { Id: productId }, function (brands) {
                                parent.$('#brands').html(brands);
                                parent.$.featherlight.close();
                            });
                        });
                    }
                });
        }
    });

});