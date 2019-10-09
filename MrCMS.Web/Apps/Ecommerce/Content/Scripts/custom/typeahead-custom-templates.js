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
                url: targetControllerUrl+ '?',
                replace: function (url, uriEncodedQuery)
                {
                    val = $('input#searchTerm').val();

                    if (!val)
                    {
                        return url.replace("%QUERY", uriEncodedQuery);
                    }

                    return url.replace("%QUERY", uriEncodedQuery) + '&searchTerm=' + encodeURIComponent(val)
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
    $('#custom-templates .typeahead').typeahead(
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
                    '<div class="imageDisplayUrl-container"><img src="{{ImageDisplayUrl}}" alt="{{ProductName}}" /></div>'+
                    '<div class="product-name-container"> {{ProductName}} </div>' +
                    '<div class="price-container"> {{ProductPrice}} </div>' +
                    '</div>')
            },
            limit: 10
        }).on('typeahead:selected', function (evt, datum)
        {
            $('#searchTerm').val(datum.ProductName)
            $('form#productSearchForm').attr("action", datum.AbsoluteUrl)
            $('form#productSearchForm').submit();
        })
});



