$(function () {
    $('button#close').click(function () {
        var pId = $("#ProductId").val();
        var saId = $("#Option").val();
        var noOfOptions = $("#Option option").length;
        if (noOfOptions > 0) {
            var saValue = $("#Value option:selected").text();
            if (saValue === "Other") {
                saValue = $("#new-option-value").val();
            }
            $.post('/Admin/Apps/Ecommerce/Product/AddSpecification',
                { Option: saId, Value: saValue, ProductId: pId },
                function (response) {
                    parent.$.get('/Admin/Apps/Ecommerce/Product/Specifications/' + pId, function (products) {
                        parent.$('#specification-list').replaceWith(products);
                    });
                    parent.$.fancybox.close();
                    return false;
                });
        } else {
            parent.$.fancybox.close();
            return false;
        }
        return false;
    });

    $('#Option').live("change", function () {
        var saId = $(this).val();
        $.getJSON('/Admin/Apps/Ecommerce/Product/GetSpecificationAttributeOptions',
            { specificationAttributeId: saId },
            function (response) {
                $("#Value").empty();
                for (var i = 0, len = response.length; i < len; i++)
                    $("#Value").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");
            });
    });

    $('#Value').live("change", function () {
        if ($(this).val() === "0") {;
            $("#other").show();
        } else {
            $("#other").hide();
        }
    });
})