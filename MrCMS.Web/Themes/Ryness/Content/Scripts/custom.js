$(window).load(function () {
    $('#slider').nivoSlider({ effect: 'fade', pauseTime: 8000, controlNav: false });
    $("#pikame").PikaChoose({ autoPlay: false });
    $('input, textarea').placeholder();
});

$(document).ready(function () {

    window.onload = init;

    function init() {
        setImageSize();
    }
    
    $.cookieBar({
        policyButton : true,
        policyURL : "/privacy-policy-and-cookie-info"
    });

    function setImageSize() {
        var browserWidth = $(window).width();
        if (browserWidth > 767) {
            $(".row-fluid .span3 .product-picture").each(function () {
                var currentHeight = $(this).height();
                if (currentHeight > 84)
                    $(".row-fluid .span3 .product-picture").attr("style", "height:" + currentHeight + "px !important");
            });

            $(".row-fluid .span3 .category-picture").each(function () {
                var currentHeight = $(this).height();
                if (currentHeight > 97)
                    $(".row-fluid .span3 .category-picture").attr("style", "height:" + currentHeight + "px !important");
            });
        }

        $(".row-fluid .span2 .product-picture").each(function () {
            var currentHeight = $(this).height();
            if (currentHeight > 133)
                $(".row-fluid .span2 .product-picture").attr("style", "height:" + currentHeight + "px !important");
        });
    }

    $(".mobile-nav ul").hide();

    $("#browse").click(function () {
        var elementClass = $(".mobile-nav ul").attr("class");
        if (elementClass != "rootSection") {
            $(".mobile-nav ul").addClass("rootSection");
            $(".mobile-nav ul").slideDown();
        }
    });

    $(".mobile-nav ul li a").click(function () {
        var itemName = $.trim($(this).text());
        $(".mobile-nav ul").hide();
        //$(".mobile-nav").append("<div style=\"margin-top:10px;height:20px;background:#dddddd;padding:10px;border:1px solid #000000;\" id='nav-title'><a href='#' class='back'><i class=\"icon-chevron-left\"></i></a><a href=\"#\">" + itemName + "</a></div><div class='row-fluid' id='sub-nav'><div class='span12'><ul class='sub-mobile-nav'><li><a href=''>Sub Item 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href=''>Sub Item 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href=''>Sub Item 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href=''>Sub Item 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul></div></div>");
        $(".mobile-nav").append("<div id='nav-title'><a href='#' class='back'><i class=\"icon-chevron-left\"></i></a><a href=\"#\">" + itemName + "</a></div><div class='row-fluid' id='sub-nav'><div class='span12'>" + getSubItems(itemName) + "</div></div>");
        $(".mobile-nav").attr("style", "margin-left:100%;");
        $(".mobile-nav").animate({ marginLeft: "0%" }, 500);
    });

    $(document).on("click", "#nav-title a.back", function () {
        $("#nav-title").remove();
        $("#sub-nav").remove();
        $(".mobile-nav ul").attr("style", "margin-right:100%;");
        $(".mobile-nav ul").animate({ marginRight: "0%" }, 200);
    });

    function getSubItems(itemName) {
        if (itemName == "Light Bulbs")
            return "<ul class='sub-mobile-nav'><li><a href='category.html'>Light Bulbs 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Light Bulbs 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Light Bulbs 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Light Bulbs 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
        else if (itemName == "Lighting")
            return "<ul class='sub-mobile-nav'><li><a href='category.html'>Lighting 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Lighting 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Lighting 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Lighting 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
        else
            return "<ul class='sub-mobile-nav'><li><a href='category.html'>Sub Item 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Sub Item 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Sub Item 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Sub Item 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
    }

    function getSecondLevelSubItems(subItem) {
        if (subItem == "Light Bulbs 1")
            return "<div style='background:red;border:1px solid black;height:20px;line-height:20px;'>Third Level Navigation</div>";
    }

    //$(document).on("click", "ul.sub-mobile-nav li a", function () {
    //    var subItemName = $.trim($(this).text());
    //    alert(subItemName);
    //    $(".mobile-nav").append("<div style='background:red;border:1px solid black;height:20px;line-height:20px;'>" + subItemName + "</div>");
    //    $(".mobile-nav").attr("style", "height:300px");
    //});

    $(".tablet-menu ul li a").click(function () {
        var itemName = $.trim($(this).text());
        $(".tablet-menu ul").hide();
        $(".tablet-menu").append("<div id='tablet-nav-title'><a href='#' class='back'><i class=\"icon-chevron-left\"></i></a><a href=\"#\">" + itemName + "</a></div><div class='row-fluid' id='tablet-sub-nav'><div class='span12'>" + getTabletSubItems(itemName) + "</div></div>");
        //$(".tablet-menu").attr("style", "position:fixed;top:90px;left:22%;");
        //$(".tablet-menu").animate({ "left": "0%" }, "fast");
        $(".tablet-menu").show();
        return false;
    });

    $(document).on("click", "#tablet-nav-title a.back", function () {
        $("#tablet-nav-title").remove();
        $("#tablet-sub-nav").remove();
        $(".tablet-menu ul").attr("style", "margin-right:100%;");
        $(".tablet-menu ul").animate({ marginRight: "0%" }, 200);
    });

    function getTabletSubItems(itemName) {
        if (itemName == "Light Bulbs")
            return "<ul class='sub-tablet-nav'><li><a href='category.html'>Light Bulbs 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Light Bulbs 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Light Bulbs 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Light Bulbs 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
        else if (itemName == "Lighting")
            return "<ul class='sub-tablet-nav'><li><a href='category.html'>Lighting 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Lighting 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Lighting 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Lighting 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
        else
            return "<ul class='sub-tablet-nav'><li><a href='category.html'>Sub Item 1 <i class=\"icon-chevron-right\"></i></a></li><li><a href='category.html'>Sub Item 2 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Sub Item 3 <i class=\"icon-chevron-right\"></i></a></a></li><li><a href='category.html'>Sub Item 4 <i class=\"icon-chevron-right\"></i></a></a></li></ul>";
    }

    $(".navbar-inverse .nav > li ul li").hover(function () {
        $(this).children("a").addClass("active");
        var currentUlHeight = $(this).closest("ul").height();
        var nextUl = $(this).find("ul");
        if (nextUl.length) {
            var nextUlHeight = nextUl.height();
            if (currentUlHeight > nextUlHeight)
                nextUl.css("height", currentUlHeight);
            else if (nextUlHeight > currentUlHeight)
                nextUl.css("height", nextUlHeight);
        }
    }, function () {
        $(this).children("a").removeClass("active");
    });

    $(".navbar-inverse .nav > li ul").hover(function () {
        $(this).parent("li").addClass("active");
    }, function () {
        $(this).parent("li").removeClass("active");
    });

    $(".navbar-inverse .nav > li ul li ul").hover(function () {
        $(this).parent("li").children("a").addClass("active");
    }, function () {
        $(this).parent("li").children("a").removeClass("active");
    });
});