$(document).ready(function ()
{    
    var productsArray = [];
    var targetControllerUrl = "search/ProductSuggestion";
    
    var productsBh = new Bloodhound(
        {
            datumTokenizer: function (datum)
            {
                return Bloodhound.tokenizers.whitespace(datum.ProductName);
            },
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: //get suggestion input data from the server (i.e Product Search Controller)
            {                
                wildcard: '%QUERY',
                url: targetControllerUrl + '?',
                replace: function (url, uriEncodedQuery)
                {
                    currentSearchTerm = $('input#searchTerm').val();
                    currentSearchTermMobile = $('input#searchTerm-mobile').val();

                    var searchTerm = (!currentSearchTerm) ? currentSearchTermMobile : currentSearchTerm;

                    if (!searchTerm)
                    {
                        return url.replace("%QUERY", uriEncodedQuery);
                    }

                    return url.replace("%QUERY", uriEncodedQuery) + '&searchTerm=' + encodeURIComponent(searchTerm);
                },
                transform: function (response) {

                //clear the array if previously populated
                productsArray.length = 0;

                for (var index in response)
                {                                
                    var productDetails = '{"ProductName": "' + response[index].ProductName + '", ' +
                                         '"ProductPrice": "' + response[index].ProductPrice + '", ' +
                                         '"AbsoluteUrl": "' + response[index].AbsoluteUrl + '", ' +
                                         '"ImageDisplayUrl": "' + response[index].ImageDisplayUrl + '"}';

                    productsArray.push(JSON.parse(productDetails)); 
                }
                                                         
                    // Map the remote source JSON array to a JavaScript object array
                    return $.map(productsArray, function (product) {
                        return {
                                    ProductName: product.ProductName,
                                    ProductPrice: product.ProductPrice, 
                                    AbsoluteUrl: product.AbsoluteUrl,
                                    ImageDisplayUrl: product.ImageDisplayUrl
                               };
                    });
                }
            }           
        });       


    // Initializing the typeahead
    $('#product-search-typeahead .typeahead').typeahead(
        {
            hint: true,
            highlight: true, // Enable substring highlighting 
            minLength: 1 // Specify minimum characters required for showing suggestions
        },
        {
            name: 'products', // The name of the dataset, used for .data attribute value setting
            displayKey: "ProductName",
            source: productsBh,
            templates:
            {
                empty: [
                    '<div class="empty-message">',
                    'unable to find any product that match the current query',
                    '</div>'
                ].join('\n'),

                suggestion: Handlebars.compile('<div class="search-options-list">' +
                    '<a href="{{AbsoluteUrl}}" >' +
                        '<div id="suggested-product-row" class="row">'+
                            '<div class="col-sm-2">'+
                            '<div class="imageDisplayUrl-container"><img src="{{ImageDisplayUrl}}" alt="{{ProductName}}" /></div>' +
                            '</div>' +
                            '<div class="col-sm-7">' +
                            '<div class="product-name-container"> {{ProductName}} </div>' +
                            '</div>' +
                            '<div class="col-sm-3">' +
                            '<div class="price-container"> {{ProductPrice}} </div>' +
                            '</div>' +
                        '</div>' +
                    '</a>'+
                    '</div>')
            },
            limit: 10
        }).on('typeahead:selected', function (evt, datum)
        {
            var formtoSubmit = $('form#productSearchForm');

                formtoSubmit.attr("action", datum.AbsoluteUrl);                    
  
                formtoSubmit.submit();          
        })



    // Initializing the typeahead for mobile
    $('#product-search-typeahead-mobile .typeahead').typeahead(
        {
            hint: true,
            highlight: true, // Enable substring highlighting 
            minLength: 1 // Specify minimum characters required for showing suggestions
        },
        {
            name: 'products', // The name of the dataset, used for .data attribute value setting
            displayKey: "ProductName",
            source: productsBh,
            templates:
            {
                empty: [
                    '<div class="empty-message">',
                    'unable to find any product that match the current query',
                    '</div>'
                ].join('\n'),
                suggestion: Handlebars.compile('<a href="{{AbsoluteUrl}}" >' +  
                    '<div class="search-options-list-mobile">' +                                                       
                            '<div class="imageDisplayUrl-container"><img src="{{ImageDisplayUrl}}" alt="{{ProductName}}" /></div>' +                                                      
                            '<div class="product-name-container"> {{ProductName}} </div>' +                                                      
                            '<div class="price-container"> {{ProductPrice}} </div>' +                  
                     '</div>'+
                    '</a>')
            },
            limit: 10
        }).on('typeahead:selected', function (evt, datum)
        {
            var formtoSubmit = $('form#productSearchForm-mobile');

                formtoSubmit.attr("action", datum.AbsoluteUrl);

                formtoSubmit.submit();   
        })
});



