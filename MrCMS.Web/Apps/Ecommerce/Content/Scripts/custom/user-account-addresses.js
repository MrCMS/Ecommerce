(function($) {
    
    function deleteAddress(event) {
        event.preventDefault();
        var val = $(event.currentTarget).data("delete-address");
        var message = $("[data-delete-message]").text();

        bootbox.confirm(message, function (result) {
            if (result) {
                postToUrl("/user-account/handle/delete-address/" + val);
            }
        });
    }

    function postToUrl(path, params) {
        var tempForm = document.createElement("form");
        tempForm.setAttribute("method", "post");
        tempForm.setAttribute("action", path);

        for (var key in params) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);
            tempForm.appendChild(hiddenField);
        }
        document.body.appendChild(tempForm);
        tempForm.submit();
    }

    $(function () {
        $(document).on("click", "[data-delete-address]", deleteAddress);
    });
    
})(jQuery);