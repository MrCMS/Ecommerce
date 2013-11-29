$(window).load(function () {
    InitializeSliders.nivoSlider();
    InitializeSliders.PikaChoose();
});

var InitializeSliders = new function () {
    this.nivoSlider = function () {
        $('#slider').nivoSlider({ effect: 'fade', pauseTime: 8000, controlNav: false });

    };
    this.PikaChoose = function () {
        $("#pikame").PikaChoose({ autoPlay: false });
    };
};

$(document).ready(function () {

    window.onload = init;

    function init() {
        setImageSize();
    }

    scrollToElement();
    function scrollToElement() {
        var browserWidth = $(window).width();
        var page = window.location.pathname;
        if (browserWidth < 767 && page != "/") {
            $('html, body').animate({ scrollTop: $('#mobileStart').offset().top }, 500);
        }
    }

    $.cookieBar({
        message: "Ryness use cookies to improve your shopping experience.",
        acceptText: "Dismiss",
        policyButton: true,
        policyText: "Find out more",
        policyURL: "/privacy-policy-and-cookie-info",
        fixed: true
    });


    function setImageSize() {
        var browserWidth = $(window).width();
        if (browserWidth > 767) {
            //get max div height
            var heights = $(".row-fluid .span3 .product-picture").map(function () {
                return $(this).height();
            }).get(), maxHeight = Math.max.apply(null, heights);
            //set all divs height to the max
            $(".row-fluid .span3 .product-picture").each(function () {
                //var currentHeight = $(this).height();
                //if (currentHeight > 84)
                $(".row-fluid .span3 .product-picture").attr("style", "height:" + maxHeight + "px !important");
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

    if (Product) {
        Product.onChangeVariant = InitializeSliders.PikaChoose;
    }
});