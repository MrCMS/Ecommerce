var AddSpecifications = function () {
    var updateCallingPage = function (event) {
        event.preventDefault();
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
                        parent.$.fancybox.close();
                    });
                });
        } else {
            parent.$.fancybox.close();
        }
    };
    var updateValues = function (event) {
        event.preventDefault();
        $("#other").hide();
        var saId = $(this).val();
        $.getJSON('/Admin/Apps/Ecommerce/Product/GetSpecificationAttributeOptions',
            { specificationAttributeId: saId },
            function (response) {
                $("#Value").empty();
                for (var i = 0, len = response.length; i < len; i++) {
                    $("#Value").append("<option value=" + response[i].Value + ">" + response[i].Text + "</option>");
                }

                $("#Value").append("<option value='0'>Other</option>");

            });
    };
    var showHideOther = function (event) {
        if ($(this).val() === "0") {
            $("#other").show();
        } else {
            $("#other").hide();
        }
    };
    return {
        init: function () {
            $(document).on('click', 'button#close', updateCallingPage);
            $(document).on('change', '#Option', updateValues);
            $(document).on('change', '#Value', showHideOther);
        }
    };
};
$(function () {
    new AddSpecifications().init();
});