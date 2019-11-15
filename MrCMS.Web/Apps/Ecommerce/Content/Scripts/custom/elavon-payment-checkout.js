
var request_method = $(this).attr("GET");
var serviceUrl = document.getElementById('ServiceUrl').value;
var targetControllerUrl = "Apps/Ecommerce/Confirm/PaymentRequestJson";
var responseEndpoint = "Apps/Ecommerce/Elavon/Notification";

$.getJSON(targetControllerUrl, function (jsonFromRequestEndpoint) {

    RealexHpp.setHppUrl(serviceUrl);

    var jsonObj = JSON.parse(jsonFromRequestEndpoint);

    RealexHpp.lightbox.init("payButtonId", responseEndpoint, jsonObj);
});