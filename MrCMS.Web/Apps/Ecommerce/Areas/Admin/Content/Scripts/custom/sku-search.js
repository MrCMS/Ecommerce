$(function () {
    reTagIt();

    function reTagIt() {
        var namesInput = $("#SKUNames");
        if (namesInput !== undefined && namesInput.val() != undefined) {
            var tags = namesInput.val().split(',');
            for (var i = 0; i < tags.length; i++) {
                namesInput.tagit().tagit("createTag", tags[i]);
            }
        }
    }

    $(document).on('keyup', '#skusearchparam', function () {
        var term = $(this).val();
        if (term !== "") {
            $.get('/Admin/ItemHasSKU/SearchSKUs/', {
                page: 1,
                query: term
                }, function (response) {
                    $('.modal-body-container').html(response);
                });
        }
    });

    $(document).on('click', '#skus .add-sku', function () {
        var button = $(this);
        var skuId = button.data('sku-id');
        console.log(skuId);
        var skuName = button.data('sku-name');
        $("#SKUNames").tagit().tagit("createTag", skuName.replace(",", ""));
        $("#SKUs").tagit().tagit("createTag", skuId);
        return false;
    });
})