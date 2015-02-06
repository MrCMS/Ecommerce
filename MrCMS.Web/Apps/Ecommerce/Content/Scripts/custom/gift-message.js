(function ($) {
    function getRemaining(textArea) {
        var length = textArea.val().length;
        var number = textArea.parents('[data-message-container]').data('message-container');
        return number - length;
    }

    function getCurrentValue(textArea) {
        return {
            message: textArea.val()
        };
    };
    function getCharacterCountMessage(textArea) {
        return $('[data-gift-message-remaining="' + textArea.attr('id') + '"]');
    };
    var timer = 0;
    function delayedUpdateGiftMessage(event) {
        var textArea = $(event.target);
        showRemaining(textArea);

        clearTimeout(timer);
        timer = setTimeout(function () { updateGiftMessage(event); }, 300);

    };

    function updateGiftMessage(event) {
        event.preventDefault();
        var textArea = $(event.target);
        $.post('/Apps/Ecommerce/SaveGiftMessage', getCurrentValue(textArea));
    };

    function showRemaining(textArea) {
        var remaining = getRemaining(textArea);
        var characterCountMessage = getCharacterCountMessage(textArea);
        characterCountMessage.html($('<span>').html(remaining + ' characters remaining'));
        var isValid = remaining >= 0;
        if (isValid) {
            characterCountMessage.removeClass('field-validation-error');
        } else {
            characterCountMessage.addClass('field-validation-error');
        }
        return isValid;
    }
    function initializeShowRemaining() {
        $('[data-gift-message]').each(function (index, element) {
            var textArea = $(element);
            var characterCountMessage = getCharacterCountMessage(textArea);
            if (!characterCountMessage.data('initialized')) {
                showRemaining(textArea);
                characterCountMessage.data('initialized', true);
            }
        });
    }

    $(function () {
        $(document).on('keyup', '[data-gift-message]', delayedUpdateGiftMessage);
        $(document).on('ajaxSuccess', initializeShowRemaining);

    });
})(jQuery);