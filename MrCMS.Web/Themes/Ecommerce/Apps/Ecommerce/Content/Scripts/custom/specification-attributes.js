$(function () {
    $('button#close').click(function () {
        pId = $("#ProductId").val();
        saId = $("#Option").val();
        noOfOptions = $("#Option option").length;
        if (noOfOptions > 0) {
            saValue = $("#Value option:selected").text();
            $.post('/Admin/Apps/Ecommerce/Product/AddSpecification',
                { Option: saId, Value: saValue, ProductId: pId },
                function (response) {
                    parent.$.get('/Admin/Apps/Ecommerce/Product/Specifications/' + pId, function (products) {
                        parent.$('#specification-list').replaceWith(products);
                    });
                    parent.$.fancybox.close();
                    return false;
                });
        }
        else {
            parent.$.fancybox.close();
            return false;
        }
    });

    $('#Option').live("change", function () {
        saId = $(this).val();
        $.getJSON('/Admin/Apps/Ecommerce/Product/GetSpecificationAttributeOptions',
            { specificationAttributeId: saId },
            function (response) {
                $("#Value").empty();
                for (var i = 0, len = response.length; i < len; i++)
                    $("#Value").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");
            });
    });
})