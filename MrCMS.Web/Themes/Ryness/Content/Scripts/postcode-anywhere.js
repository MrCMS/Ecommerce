function PostcodeAnywhere_Interactive_Find_v1_10Begin(Key, SearchTerm, PreferredLanguage, Filter, UserName) {

    if (SearchTerm == "Enter postcode" || jQuery.trim(SearchTerm) == "") {
        var errorMessage = $('#ShippingAddress_txtErrorMessage').val();

        alert(errorMessage);
    }
    else {
        var scriptTag = document.getElementById("PCA6d35cfc188f1451f9cfdf1b5d751a716");
        var headTag = document.getElementsByTagName("head").item(0);
        var strUrl = "";

        //Build the url
        strUrl = "https://services.postcodeanywhere.co.uk/PostcodeAnywhere/Interactive/Find/v1.10/json.ws?";
        strUrl += "&Key=" + encodeURI(Key);
        strUrl += "&SearchTerm=" + encodeURI(SearchTerm);
        strUrl += "&PreferredLanguage=" + encodeURI(PreferredLanguage);
        strUrl += "&Filter=" + encodeURI(Filter);
        strUrl += "&UserName=" + encodeURI(UserName);
        strUrl += "&CallbackFunction=PostcodeAnywhere_Interactive_Find_v1_10End";

        //Make the request
        if (scriptTag) {
            try {
                headTag.removeChild(scriptTag);
            }
            catch (e) {
                //Ignore
            }
        }
        scriptTag = document.createElement("script");
        scriptTag.src = strUrl;
        scriptTag.type = "text/javascript";
        scriptTag.id = "PCA6d35cfc188f1451f9cfdf1b5d751a716";
        headTag.appendChild(scriptTag);
    }

}

function PostcodeAnywhere_Interactive_Find_v1_10End(response) {
    //Test for an error
    if (response.length == 1 && typeof (response[0].Error) != 'undefined') {
        //Show the error message
        var returnErrorMessage = response[0].Description;
        var customErrorMessage = $('#ShippingAddress_txtOutofCredit').val();

        if (returnErrorMessage == "Account out of credit")
            alert(customErrorMessage);
        else
            alert(returnErrorMessage);
    }
    else {
        //Check if there were any items found
        if (response.length == 0) {
            var errorMessage = $('#ShippingAddress_txtErrorMessage').val();

            alert(errorMessage);
        }
        else {

            for (i = document.getElementById('return').options.length - 1 ; i >= 0; i--) {
                document.getElementById('return').options.remove(i);
            }

            document.getElementById('postcode-lookup-container').style.display = '';
            document.getElementById('return').options.add(new Option("Select Address:", "0"));

            for (var i = 0; i < response.length + 1; i++)
                document.getElementById('return').options.add(new Option(response[i].StreetAddress + ", " + response[i].Place, response[i].Id));
        }
    }
}

function PostcodeAnywhere_Interactive_RetrieveById_v1_10Begin(Key, Id, PreferredLanguage, UserName) {
    var scriptTag = document.getElementById("PCAa73f9bc2b60d4e4cbd595512478a3291");
    var headTag = document.getElementsByTagName("head").item(0);
    var strUrl = "";

    //Build the url
    strUrl = "https://services.postcodeanywhere.co.uk/PostcodeAnywhere/Interactive/RetrieveById/v1.10/json.ws?";
    strUrl += "&Key=" + encodeURI(Key);
    strUrl += "&Id=" + encodeURI(Id);
    strUrl += "&PreferredLanguage=" + encodeURI(PreferredLanguage);
    strUrl += "&UserName=" + encodeURI(UserName);
    strUrl += "&CallbackFunction=PostcodeAnywhere_Interactive_RetrieveById_v1_10End";

    //Make the request
    if (scriptTag) {
        try {
            headTag.removeChild(scriptTag);
        }
        catch (e) {
            //Ignore
        }
    }
    scriptTag = document.createElement("script");
    scriptTag.src = strUrl
    scriptTag.type = "text/javascript";
    scriptTag.id = "PCAa73f9bc2b60d4e4cbd595512478a3291";
    headTag.appendChild(scriptTag);
}

function PostcodeAnywhere_Interactive_RetrieveById_v1_10End(response) {
    //Test for an error
    if (response.length == 1 && typeof (response[0].Error) != 'undefined') {
        //Show the error message
        alert(response[0].Description);
    }
    else {
        //Check if there were any items found
        if (response.length == 0) {
            var errorMessage = $('#ShippingAddress_txtErrorMessage').val();

            alert(errorMessage);
        }
        else {
            /* Here to add result*/
            var retstring = response[0].Company + "<br>" + response[0].Line1 + "<br>" + response[0].Line2 + "<br>" + response[0].Line3 + "<br>" + response[0].PostTown + "<br>" + response[0].County + "<br>" + response[0].Postcode;

            var address1 = response[0].Line1;
            var address2 = response[0].Line2;
            var city = response[0].PostTown;
            var zipPostalCode = response[0].Postcode;
            var countyName = response[0].County;
            var companyName = response[0].Company;

            if (city == "London") {
                countyName = "Greater London";
            }

            //set values in form
            $('#Address1').val(address1);
            $('#Address2').val(address2);
            $('#City').val(city);
            $('#PostalCode').val(zipPostalCode);
            $('#Company').val(companyName);
            $('#County').val(countyName);
        }
    }
}