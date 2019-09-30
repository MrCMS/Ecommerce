
    // Custom styling can be passed to options when creating an Element.
    var style =
    {
        base:
        {
            // Add your base input styles here. For example:
            fontSize: '16px',
            color: "#32325d",
            border: "none!important",
            margin: "0px !important",
            padding:"0px !important"
        }
    };

    //retrive apikey value from stripe settings
    var apiKeyInputElement = document.getElementById('PublicKey');

    //create an instance of the Stripe object
    var stripe = Stripe(apiKeyInputElement.value);

    //create an instance of the Elements object
    var elements = stripe.elements();

    // Create an instance of the card Element.
    var cardElement = elements.create('card', {style: style });

    // Add an instance of the card Element into the `card-element` <div>.
    cardElement.mount('#card-element');

    var cardholderName = document.getElementById('CustomerName');
    var cardButton = document.getElementById('card-button');
    var clientSecret = cardButton.dataset.secret;

    var postalCodeElement = document.getElementById('PostalCode').value;
    var lineOne = document.getElementById('LineOne').value;
    var lineTwo = document.getElementById('LineTwo').value;
    var userCity = document.getElementById('City').value;
    var userCountry = document.getElementById('Country').value;
    var userState = document.getElementById('State').value;


$(document).ready(function () {
    $("input[name='exp-date']").focusin(function () {
        alert("exp date got focus");
        $("input[name='postal']").setAttribute("value", postalCodeElement);
    });
});

    //Handle Payment Form sudmit event to Stripe from the client
    var form = document.getElementById('payment-form');    
        form.addEventListener('submit', function (event)
        {
            //delay form submission by stopping default behaviour
            event.preventDefault();

            //disable the submit button to prevent multiple submissions 
            //of the same order / purchase request
            cardButton.disabled = true;
            

            //complete the Stripe card payment
            stripe.handleCardPayment(
                clientSecret,
                cardElement,
                {
                    payment_method_data:
                    {
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
            ).then(function (result)
                {
                    if (result.error)
                    {                        
                        // Display error.message in the UI so that the user is 
                        // informed about the  error occurred.
                        var errorElement = document.getElementById('card-errors');
                            errorElement.textContent = result.error.message;

                        //reset the card input elements so that the user can retry
                        $.ajax({
                            url: 'Apps/Ecommerce/Confirm/Stripe',
                            contentType: 'application/html; charset=utf-8',
                            type: 'GET',
                            dataType: 'html'
                        }).success(function (result)
                            {
                                $('#card-element').html(result);
                            })
                            .error(function (xhr, status)
                            {
                                alert(status);
                            })  

                        //enable the submit button so that the user can retry
                        cardButton.disabled = false;                     
                      }
                    else
                    {
                        // The payment has succeeded. Display a success message.
                        var status = result.paymentIntent.status ? result.paymentIntent.status : result.error.message;
                        var hiddenCardPaymentStatusInput = document.getElementById('HandleCardPaymentStatus');
                            hiddenCardPaymentStatusInput.setAttribute('value', status);

                        //submit the form after the aditional waiting time set
                        //in the callback function inside setTimeout
                        setTimeout(function()
                        {
                           form.submit();
                        }, 500);
                    }
               });                                
        });  

