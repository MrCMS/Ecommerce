
// Custom styling can be passed to options when creating an Element.
var style =
{
    base:
    {
        // Add your base input styles here. For example:
        fontSize: '16px',
        color: "#32325d"
    }
};

// retrieve apiKey value from stripe settings
var apiKeyInputElement = document.getElementById('PublicKey');

//create an instance of the Stripe object
var stripe = Stripe(apiKeyInputElement.value);

//create an instance of the Elements object
var elements = stripe.elements();

// Create an instance of the card Element.
var cardElement = elements.create('card', { style: style, hidePostalCode: true });

// Add an instance of the card Element into the "card-element" <div>.
cardElement.mount('#card-element');

var cardholderName = document.getElementById('CustomerName');
var cardButton = document.getElementById('card-button');
var clientSecret = cardButton.dataset.secret;

// Customer address details
var postalCodeElement = document.getElementById('PostalCode').value;
var lineOne = document.getElementById('LineOne').value;
var lineTwo = document.getElementById('LineTwo').value;
var userCity = document.getElementById('City').value;
var userCountry = document.getElementById('Country').value;
var userState = document.getElementById('State').value;

//Handle Payment Form submit event to Stripe from the client
var form = document.getElementById('payment-form');

form.addEventListener('submit', function (event) {
    //delay form submission by stopping default behaviour
    event.preventDefault();

    //disable the submit button to prevent multiple submissions 
    //of the same order / purchase request
    cardButton.disabled = true;

    stripe.confirmCardPayment(
        //'{PAYMENT_INTENT_CLIENT_SECRET}',
        clientSecret,
        {
            payment_method: {
                card: cardElement,
                billing_details:
                {
                    name: cardholderName.value,
                    address:
                    {
                        line1: lineOne,
                        line2: lineTwo,
                        city: userCity,
                        country: userCountry,
                        postal_code: postalCodeElement,
                        state: userState
                    }
                }
            }
        }
    ).then(function (cardPaymentResult) {

        // Display error message in the UI so that the user is
        // informed about the  error occurred.
        var errorElement = document.getElementById('card-errors');

        if (cardPaymentResult.error) {
            //Payment error alert message
            errorElement.textContent = cardPaymentResult.error.message;

            //reset the card input elements so that the user can retry
            $.ajax({
                url: 'Apps/Ecommerce/Confirm/Stripe',
                contentType: 'application/html; charset=utf-8',
                type: 'GET',
                dataType: 'html'
            }).done(function (ajaxResult) {
                $('#card-element').empty();
                $('#card-element').html(ajaxResult);

                var reloadedCardButton = document.getElementById('card-button');

                //enable the submit button so that the user can retry
                reloadedCardButton.disabled = false;
                reloadedCardButton.setAttribute('value', 'Confirm Order');


            }).error(function (xhr, status) {
                //Payment(card details) form reload error alert message
                errorElement.textContent = "Error reloading card details form";
            });
        }
        else {
            // The payment has succeeded. Display a success message.
            var status = cardPaymentResult.paymentIntent.status ? cardPaymentResult.paymentIntent.status : cardPaymentResult.error.message;

            var hiddenCardPaymentStatusInput = document.getElementById('HandleCardPaymentStatus');
            hiddenCardPaymentStatusInput.setAttribute('value', status);

            //submit the form after the additional waiting time set
            //in the callback function inside setTimeout
            setTimeout(function () {
                form.submit();
            }, 500);
        }
    });
});

