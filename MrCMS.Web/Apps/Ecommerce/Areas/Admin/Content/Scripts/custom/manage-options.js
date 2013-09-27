var ManageOptions = new function () {
    this.init = function () {
        $('#product-option').change(function () {
            var val = $(this).val();
            if (val == -1) {
                $('#new-product-option').show();
                $('#add-product-option').hide();
            } else {
                $('#new-product-option').hide();
                $('#add-product-option').show();
            }
        });
        $('#add-product-option').click(function (event) {
            event.preventDefault();
            var value = $('#product-option').val();
            if (value == -1 || value == "")
                return;
            ManageOptions.assignOption(value);
        });
        $('#add-new-product-option').click(function (event) {
            event.preventDefault();
            if ($('#Name').val() == "")
                return;
            var $this = $(this);
            var form = $this.parents('form');
            if (form.data('validator').valid()) {
                $.post(form.attr('action'), form.serialize(), function (id) {
                    ManageOptions.assignOption(id);
                });
            }
        });
        $('[data-action="remove-option"]').click(function () {
            if (confirm('Are you sure you want to remove this option?')) {
                var $this = $(this);
                var optionId = $this.data('id');
                var productId = $this.data('product-id');
                $.post('/Admin/Apps/Ecommerce/Product/RemoveProductOption/' + productId, { productOptionId: optionId }, function () {
                    ManageOptions.resetPage();
                });
            }
        });

        $.validator.unobtrusive.parse('form');
        $('#product-option').change();
    };
    this.resetPage = function () {
        var productId = $('#product-id').val();
        $.get('/Admin/Apps/Ecommerce/Product/AddProductOption/' + productId, function (addOption) {
            $('#product-option-form').replaceWith(addOption);
            $.get('/Admin/Apps/Ecommerce/ProductOption/Add', function (addNewOption) {
                $('#add-new-option-form').replaceWith(addNewOption);
                $.get('/Admin/Apps/Ecommerce/Product/ProductOptions/' + productId, function (productOptions) {
                    $('#product-option-list').replaceWith(productOptions);
                    ManageOptions.init();
                    parent.$.get('/Admin/Apps/Ecommerce/Product/PricingInfo/' + productId, function(info) {
                        parent.$('#product-pricing-info').replaceWith(info);
                    });
                });
            });
        });
    };
    this.assignOption = function (value) {
        var form = $('#product-option-form');

        $.post(form.attr('action'), { productOptionId: value }, function () {
            ManageOptions.resetPage();
        });
    };
};
$(function () {
    ManageOptions.init();
})