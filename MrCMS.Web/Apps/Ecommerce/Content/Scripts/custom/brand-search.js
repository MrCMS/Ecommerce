(function ($, window) {
    'use strict';
    var history = window.History;
    $(function () {
        $(document).on('change', '#brand-search-container input, #brand-search-container select', onElementChange);
        $(document).on('click', '#brand-results-container .pagination a', changePage);

        $(document).on('click', '#brand-query-container a[data-action=remove-specification]', removeSpecification);
        $(document).on('click', '#brand-query-container a[data-action=remove-option]', removeOption);

        // Bind to StateChange Event
        history.Adapter.bind(window, 'statechange', updatePage);

        initializeSlider();
    });

    function onElementChange() {
        var data = getData(1);
        history.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
    }

    function getPage(href) {
        var querystring = href.split('?')[1];
        var parts = querystring.split('&');
        return $.grep(parts, keyIsPage)[0].split('=')[1];
    }

    function keyIsPage(el, index) {
        return el.split('=')[0] == "page";
    }

    function initializeSlider() {
        $("#slider-range").slider({
            range: true,
            max: $('#MaxPrice').val(),
            step: 5,
            values: [parseFloat($('#PriceFrom').val()), parseFloat($('#PriceTo').val())],
            slide: updateSliderAmount,
            change: onElementChange
        });
        $("#amount").text("£" + parseFloat($("#slider-range").slider("values", 0)).toFixed(2) +
            " - £" + parseFloat($("#slider-range").slider("values", 1)).toFixed(2));
    }

    function updateSliderAmount(event, ui) {
        var from = parseFloat(ui.values[0]);
        var to = parseFloat(ui.values[1]);
        $("#amount").text("£" + from.toFixed(2) + " - £" + to.toFixed(2));
    }

    function getValue(index, element) {
        var val = $(element).val();
        if (val == '' || val == undefined)
            return null;
        return val;
    }

    function getData(page) {
        var specifications = $('#brand-query-container select[name="Specifications"], #brand-query-container input[name="Specifications"]').map(getValue).toArray();
        var options = $('#brand-query-container select[name="Options"], #brand-query-container input[name="Options"]').map(getValue).toArray();
        var sortBy = $('#SortBy').val();
        var pageSize = $('#PageSize').val();
        var categoryId = $('#CategoryId').val();
        var brandId = $('#Id').val();
        var searchTerm = $('#searchTerm').val();

        return {
            Specifications: specifications.join('|'),
            Options: options.join('|'),
            SortBy: sortBy,
            PageSize: pageSize,
            CategoryId: categoryId,
            PriceFrom: getSliderFromValue(),
            PriceTo: getSliderToValue(),
            BrandId: brandId,
            SearchTerm: searchTerm,
            Page: page
        };
    }

    function getSliderFromValue() {
        var value = $("#slider-range").slider("values", 0);
        return value == 0 ? null : value;
    }

    function getSliderToValue() {
        var value = $("#slider-range").slider("values", 1);
        return value == $('#MaxPrice').val() ? null : value;
    }

    function buildUpQueryString(data) {
        var sortByDefault = $('#SortBy option').eq(0).val();
        var pageSizeDefault = $('#PageSize option').eq(0).val();
        if (data.Specifications.length == 0 && data.Options.length == 0
            && data.SortBy == sortByDefault
            && data.PageSize == pageSizeDefault
            && data.PriceFrom == null
            && data.PriceTo == null
            && (data.SearchTerm == '' || data.SearchTerm == undefined)
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
        if (data.SearchTerm != '' && data.SearchTerm != undefined) {
            if (queryString.length > 0) queryString += "&";
            queryString += "SearchTerm=" + data.SearchTerm;
        }
        if (data.Page > 1) {
            if (queryString.length > 0) queryString += "&";
            queryString += "Page=" + data.Page;
        }

        return "?" + queryString;
    }

    function changePage(event) {
        event.preventDefault();
        var element = $(event.target);
        if (element.attr('href') === undefined)
            return;
        var page = getPage(element.attr('href'));
        var data = getData(page);

        history.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
    }

    function removeSpecification(event) {
        event.preventDefault();
        var element = $(event.currentTarget);
        element.siblings('input[name=Specifications]').remove();
        var data = getData(1);

        history.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
    }

    function removeOption(event) {
        event.preventDefault();
        var element = $(event.currentTarget);
        element.siblings('input[name=Options]').remove();
        var data = getData(1);

        history.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
    }

    function updatePage(event) { // Note: We are using statechange instead of popstate
        var state = history.getState();
        var data = state.data;
        data.BrandId = $('#Id').val();
        $.get('/brand/search/query', data, updateQuery);
        $("#loading-message").show();
        $.get('/brand/search/results', data, updateResults);
    }

    function updateQuery(response) {
        $('#brand-query-container').replaceWith(response);
        initializeSlider();
    }

    function updateResults(response) {
        $("#loading-message").hide();
        $('#brand-results-container').replaceWith(response);
        var container = $("[data-brand-search-container]");
        var scrollTo = container.data('brand-search-scroll-to');
        if (scrollTo) {
            var top = $(scrollTo).offset().top;
            $('html, body').animate({
                scrollTop: top
            }, 350);
        }
    }
})(jQuery, window);