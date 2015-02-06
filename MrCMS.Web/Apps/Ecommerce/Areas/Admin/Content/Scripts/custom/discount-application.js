(function ($) {
    var ddlSelector = '#ApplicationType',
        fromLimitationSelector = '#CartItemsFromLimitations',
        fieldsSelector = '[data-application-fields]',
        formSelector = '[data-discount-application]';

    function updateFancyBox() {
        parent.$.fancybox.update();
    }
    function setFields(html) {
        var fieldsContainer = $(fieldsSelector);
        fieldsContainer.html(html);
        $(fromLimitationSelector).change();
        updateFancyBox();
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
    function showHideProductFields(event) {
        var checkbox = $(event.target);
        var fields = $('[data-from-limitations]');
        if (checkbox.is(':checked'))
            fields.hide({ duration: 0, complete: updateFancyBox });
        else
            fields.show({ duration: 0, complete: updateFancyBox });
    }

    $(function () {
        $(document).on('change', ddlSelector, updateFields);
        $(document).on('change', fromLimitationSelector, showHideProductFields);
        $(document).on('submit', formSelector, submitForm);

        $(fromLimitationSelector).change();
    });
})(jQuery);