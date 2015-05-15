(function ($) {
    var client;
    var token;

    $(function () {
        $('#braintree-checkout').on('submit', check3DSecure);
        token = getClientToken();
        client = new braintree.api.Client({ clientToken: token });
    });

    function getClientToken() {
        return $('#ClientToken').val();
    }

    function getForm() {
        return $("[data-braintree-checkout-form]");
    }

    function check3DSecure(event) {
        event.preventDefault();
        var form = $(event.target);
        var errorArea = $('[data-braintree-errors]');
        var submitButton = form.find('[data-submit-button]');

        errorArea.hide();
        disablePaymentButton(submitButton);

        client.verify3DS({
            amount: form.find('#TotalToPay').val(),
            creditCard: {
                number: form.find('[data-braintree-name="number"]').val(),
                cardholderName: form.find('[data-braintree-name="cardholder_name"]').val(),
                expirationMonth: form.find('[data-braintree-name="expiration_month"]').val(),
                expirationYear: form.find('[data-braintree-name="expiration_year"]').val(),
                cvv: form.find('[data-braintree-name="cvv"]').val(),
                billingAddress: {
                    postalCode: form.find('[data-braintree-name="postal_code"]').val()
                }
            }
        }, function (error, response) {
            if (!error) {
                postToUrl("/Apps/Ecommerce/Confirm/BraintreePayment", {
                    nonce: response.nonce,
                    liabilityShifted: response.verificationDetails.liabilityShifted,
                    liabilityShiftPossible: response.verificationDetails.liabilityShiftPossible
                });
            }
            else {
                enablePaymentButton(submitButton);
                errorArea.empty();
                errorArea.show();
                errorArea.append(error.message);
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

    function disablePaymentButton(submitButton) {
        submitButton.attr('disabled', 'disabled');
        submitButton.val("Processing");
    }

    function enablePaymentButton(submitButton) {
        submitButton.removeAttr('disabled');
        submitButton.val("Confirm Order");
    }

})(jQuery);