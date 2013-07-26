$(function () {
    var History = window.History; // Note: We are using a capital H instead of a lower h
    $(document).on('change', '#product-search-container input, #product-search-container select', function () {
        var data = getData(1);
        History.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
    });

    function getPage(href) {
        var querystring = href.split('?')[1];
        var parts = querystring.split('&');
        return $.grep(parts, function (el, index) {
            return el.split('=')[0] == "page";
        })[0].split('=')[1];
    }
    $(document).on('click', '#product-results-container .pagination a', function () {
        if ($(this).attr('href') === undefined)
            return false;
        var page = getPage($(this).attr('href'));
        var data = getData(page);

        History.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
        return false;
    });

    function getData(page) {
        var specifications = $('#product-query-container select[name="Specifications"]').map(function (index, element) { return $(element).val(); }).toArray();
        var options = $('#product-query-container select[name="Options"]').map(function (index, element) { return $(element).val(); }).toArray();
        var sortBy = $('#SortBy').val();
        var pageSize = $('#PageSize').val();
        var categoryId = $('#CategoryId').val();
        var brandId = $('#BrandId').val();
        var searchTerm = $('#searchTerm').val();

        return {
            Specifications: specifications.join(','),
            Options: options.join(','),
            SortBy: sortBy,
            PageSize: pageSize,
            CategoryId: categoryId,
            PriceFrom: getFromValue(),
            PriceTo: getToValue(),
            BrandId: brandId,
            SearchTerm: searchTerm,
            Page: page
        };
    }
    function getFromValue() {
        var value = $("#slider-range").slider("values", 0);
        return value == 0 ? null : value;
    }

    function getToValue() {
        var value = $("#slider-range").slider("values", 1);
        return value == $('#MaxPrice').val() ? null : value;
    }

    function buildUpQueryString(data) {
        var sortByDefault = $('#SortBy option').eq(0).val();
        var pageSizeDefault = $('#PageSize option').eq(0).val();
        var brandDefault = $('#BrandId option').eq(0).val();
        if (data.Specifications.length == 0 && data.Options.length == 0
            && data.SortBy == sortByDefault
            && data.PageSize == pageSizeDefault
            && data.PriceFrom == null
            && data.PriceTo == null
            && data.SearchTerm == ''
            && data.BrandId == brandDefault
            && data.Page <= 1)
            return "";
        var queryString = "";
        if (data.Specifications.length > 0) {
            queryString += "Specifications=" + data.Specifications;
        }
        if (data.Options.length > 0) {
            if (queryString.length > 0) queryString += "&";
            queryString += "Options=" + data.Options;
        }
        if (data.SortBy != sortByDefault) {
            if (queryString.length > 0) queryString += "&";
            queryString += "SortBy=" + data.SortBy;
        }
        if (data.PageSize != pageSizeDefault) {
            if (queryString.length > 0) queryString += "&";
            queryString += "PageSize=" + data.PageSize;
        }
        if (data.PriceFrom != null) {
            if (queryString.length > 0) queryString += "&";
            queryString += "PriceFrom=" + data.PriceFrom;
        }
        if (data.PriceTo != null) {
            if (queryString.length > 0) queryString += "&";
            queryString += "PriceTo=" + data.PriceTo;
        }
        if (data.BrandId != brandDefault) {
            if (queryString.length > 0) queryString += "&";
            queryString += "BrandId=" + data.BrandId;
        }
        if (data.SearchTerm != '') {
            if (queryString.length > 0) queryString += "&";
            queryString += "SearchTerm=" + data.SearchTerm;
        }
        if (data.Page > 1) {
            if (queryString.length > 0) queryString += "&";
            queryString += "Page=" + data.Page;
        }

        return "?" + queryString;
    }

    // Bind to StateChange Event
    History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
        var State = History.getState();
        $.get('/search/query', State.data, function (response) {
            $('#product-query-container').replaceWith(response);
            initializeSlider();
        });
        $.get('/search/results', State.data, function (response) {
            $('#product-results-container').replaceWith(response);
            if ($(window).width() <= 767) {
                $(document).scrollTop($("#product-results-container").offset().top);
            }
        });
    });
    function initializeSlider() {
        $("#slider-range").slider({
            range: true,
            min: 0,
            max: $('#MaxPrice').val(),
            step: 5,
            values: [parseFloat($('#PriceFrom').val()), parseFloat($('#PriceTo').val())],
            slide: function (event, ui) {
                $("#amount").text("£" + ui.values[0].toFixed(2) + " - £" + ui.values[1].toFixed(2));
            },
            change: function (event, ui) {
                var data = getData(1);
                History.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
            }
        });
        $("#amount").text("£" + parseFloat($("#slider-range").slider("values", 0)).toFixed(2) +
            " - £" + parseFloat($("#slider-range").slider("values", 1)).toFixed(2));
    }

    initializeSlider();
});
