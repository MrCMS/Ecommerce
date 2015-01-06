(function($) {
    function updateCartInfo() {
        $('[data-cart-info]').each(function() {
            var info = $(this);
            $.get(info.data('cart-info'), function(result) {
                info.replaceWith(result);
            });
        });
    }

    function emptyBasket(event) {
        event.preventDefault();
        var response = confirm("Are you sure you want to empty your basket? You can not undo this action.");
        if (response === true) {
            $.post('/Apps/Ecommerce/EmptyBasket', updateCartInfo);
        }
    }

    function updateBasket(event) {
        event.preventDefault();
        var values = $('input[type="text"][name^="quantity-"]').map(function(index, element) {
            var quantityElement = $(element);
            var quantity = quantityElement.val();
            var cartId = quantityElement.data('cart-id');
            return cartId + ":" + quantity;
        }).toArray();

        $.post('/Apps/Ecommerce/UpdateBasket', { quantities: values.join(',') }, updateCartInfo);
    }

    function deleteCartItem(event) {
        event.preventDefault();
        var id = $(this).data('id');
        $.post('/Apps/Ecommerce/DeleteCartItem', { id: id }, updateCartInfo);
    }

    function toggleUseRewardPoints(event) {
        event.preventDefault();
        $.post('/Apps/Ecommerce/UseRewardPoints', { useRewardPoints: $(event.target).is(':checked') }, updateCartInfo);
    }

    $(function() {
        $.ajaxSetup({ cache: false });

        $(document).on('click', "#empty-basket", emptyBasket);
        $(document).on('click', "#update-basket", updateBasket);
        $(document).on('click', "a[data-action=delete-cart-item]", deleteCartItem);
        $(document).on('change', "#UseRewardPoints", toggleUseRewardPoints);

        $(document).on('add-discount', updateCartInfo);
        $(document).on('remove-discount', updateCartInfo);

        $(document).on('gift-card-applied', updateCartInfo);
        $(document).on('gift-card-removed', updateCartInfo);

    });
})(jQuery);