$(function () {
    $(document).on('change', "select[id='ShippingCalculation']", function () {
        updateShippingMethod();
    });
    $(document).on('click', "#continue", function () {
        if ($("#ShippingCalculation").val() !== null && $("#ShippingCalculation").val() !== "") {
            $("form#deliveryDetails").submit();
        }
        return false;
    });
    
    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/Cart/AddShippingMethod',
           { id: $("select[id='ShippingCalculation']").val() },
           function (response) {
               parent.$.get('/Apps/Ecommerce/Cart/BasicDetails', function (result) {
                   parent.$('#basic-details').replaceWith(result);
               });
               setCountry();
           });
        
    }
    function setCountry() {
        if ($("#ShippingCalculation").val() !== null && $("#ShippingCalculation").val() !== "") {
            $.post('/Apps/Ecommerce/Cart/GetShippingCalculationCountry',
                  { id: $("select[id='ShippingCalculation']").val() },
                  function (country) {
                      if (country !== false) {
                          $("select[id='Country_Id']").val(country);
                          $("select[id='Country_Id']").prop('disabled', 'disabled');
                      } else {
                          $("select[id='Country_Id']").prop('disabled', false);
                      }
                  });
        } else {
            $("select[id='Country_Id']").prop('disabled', false);
        }
    }

    setCountry();
});