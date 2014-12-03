(function ($) {
    var ddlSelector = '#ApplicationType',
        fieldsSelector = '[data-application-fields]',
        formSelector = '[data-discount-application]';

    function setFields(html) {
        var fieldsContainer = $(fieldsSelector);
        fieldsContainer.html(html);
        parent.$.fancybox.update();
    }

    function submitForm(event) {
        event.preventDefault();
        var form = $(event.target);
        $.post(form.attr('action'), form.serialize(), function (response) {
            if (response) {
                parent.$(parent.document).trigger('reload-applications');
                parent.$.fancybox.close();
            }
        });
    }

    function updateFields(event) {
        var typeSelector = $(event.target);
        var value = typeSelector.val();
        if (value) {
            var lastIndexOf = value.lastIndexOf('.');
            var typeName = value.substring(lastIndexOf + 1);
            $.get('/Admin/Apps/Ecommerce/' + typeName + '/Fields', function (response) {
                setFields(response);
            }).fail(function () {
                setFields('');
            });
        } else {
            setFields('');
        }
    }

    $(function () {
        $(document).on('change', ddlSelector, updateFields);
        $(document).on('submit', formSelector, submitForm);
    });
})(jQuery);