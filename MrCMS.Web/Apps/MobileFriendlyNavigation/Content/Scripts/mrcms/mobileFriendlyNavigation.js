(function () {
    'use strict';

    var common,
        mobile,
        desktop,
        rootSelector = '[data-mfnav="root"]',
        navSelector = '[data-mfnav="nav"]',
        subNavSelector = '[data-mfnav="subNav"]',
        navItemSelector = '[data-mfnav="item"]';

    (function () {
        $(function () {
            var $root = $(rootSelector);

            mobile.init($root);
            desktop.init($root);
            setMode($root);

            $(window).resize(function () {
                setMode($root);
            });
        });

        function setMode($root) {
            var isReponsiveLayoutDesktop = $root.width() > 767;

            if (isReponsiveLayoutDesktop) {
                if ($root.data('mfnav-mode') !== 'desktop') {
                    mobile.deactivate($root);
                    desktop.activate($root);
                }

                desktop.resize($root);
                return;
            }

            if ($root.data('mfnav-mode') !== 'mobile') {
                desktop.deactivate($root);
                mobile.activate($root);
            }
        }
    }());

    common = (function () {
        function loadSubNav($navItem, callback) {
            var url = $navItem.closest(navSelector).data('mfnav-url'),
                data = { id: $navItem.data('mfnav-id') };

            $.get(url, data, function (response) {
                var $subNav = appendNavFromJson($navItem, response);

                if (typeof (callback) === 'function') {
                    callback($subNav);
                }
            });
        }

        function appendNavFromJson($navItem, data) {
            var level = ($navItem.parent().data('mfnav-level') || 1) + 1,
                maxLevel = $navItem.closest(navSelector).data('mfnav-max-levels'),
                $subNav,
                $link;

            $subNav = $('<ul ' + subNavSelector.replace('[', '').replace(']', '') + '>')
                .addClass('sub-nav')
                .data('mfnav-level', level)
                .hide();

            $.each(data, function () {
                $link = $('<a href="{url}">{text}</a>'.supplant(this))
                    .prepend('<span class="glyphicon glyphicon-chevron-left"></span>')
                    .append('<span class="glyphicon glyphicon-chevron-right"></span>');

                $('<li ' + navItemSelector.replace("[", "").replace("]", "") + '>')
                    .append($link)
                    .data('mfnav-id', this.id)
                    .data('mfnav-has-sub-nav', this.hasChildren)
                    .addClass((this.hasChildren && level < maxLevel) ? 'has-sub-nav' : '')
                    .appendTo($subNav);
            });

            $navItem.append($subNav);

            return $subNav;
        }

        return {
            loadSubNav: loadSubNav
        };
    }());

    desktop = (function () {
        function init($root) {
            var $nav = $root.children(navSelector);

            $nav
                .on('click.desktop-mfnav', navItemSelector, onClick);

            $nav
                .find(subNavSelector)
                .addClass('dropdown-menu');

            $(document).click(function () {
                if (!$(this).hasClass(".sub-nav")) {
                    $(".sub-nav.dropdown-menu").hide();
                }
            });
        }
        function hideOtherNavs(navItem) {
            $(subNavSelector).not(navItem.parents()).hide();
        }

        function activate($root) {
            $root.data('mfnav-mode', 'desktop');
            resize($root);
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
            hideOtherNavs($root);
        }

        function resize($root) {
            var $nav = $root.children(navSelector);

            $nav.children(navItemSelector).has(subNavSelector).each(function () {
                var $navItem = $(this),
                    $subNav = $navItem.children(subNavSelector),
                    rightOffset = $(window).width() - ($navItem.offset().left + $navItem.outerWidth()),
                    align = rightOffset < ($subNav.outerWidth() * 3) ? 'left' : 'right';

                $subNav
                    .removeClass('sub-nav-left')
                    .removeClass('sub-nav-right')
                    .addClass('sub-nav-' + align);
            });
        }


        function onClick(event) {
            var $navItem = $(event.target).closest(navItemSelector),
                $subNav = $navItem.children(subNavSelector);
            if (!$navItem.hasClass("has-sub-nav")) {
                return;
            }
            event.preventDefault();
            event.stopPropagation();

            hideOtherNavs($navItem);

            if ($subNav.length) {
                showSubNav($subNav);
                return;
            }

            tryLoadSubNav($navItem);
        }

        function showSubNav($subNav) {
            var parentHeight = $subNav.parent().closest(subNavSelector).outerHeight();
            var subNavHeight = $subNav.outerHeight();
            if (parentHeight > subNavHeight) {
                $subNav.css("min-height", parentHeight + "px");
            }
            $subNav.show();
        }

        function hideSubNav($subNav) {
            $subNav.hide();
        }

        function tryLoadSubNav($navItem) {
            var level = ($navItem.parent().data('mfnav-level') || 1),
                maxLevel = $navItem.closest(navSelector).data('mfnav-max-levels'),
                hasSubNav = $navItem.data('mfnav-has-sub-nav');

            if (level < maxLevel && hasSubNav) {
                common.loadSubNav($navItem, onSubNavLoaded);
                return true;
            }

            return false;
        }

        function onSubNavLoaded($subNav) {
            $subNav.addClass('dropdown-menu');
            showSubNav($subNav);
        }

        return {
            init: init,
            activate: activate,
            deactivate: deactivate,
            resize: resize
        };
    }());

    mobile = (function () {
        var mobileNavSelector = '#mfnav-mobile',
            headerSelector = '[data-mfnav="mobileHeader"]',
            crumbsSelector = '[data-mfnav="mobileCrumbs"]';

        function init() {

            var mobileNav = $(mobileNavSelector);

            mobileNav
                .data('mfnav-route', [])
                .on('click.mfnav', navItemSelector + ' > A', onClickLink)
                .on('click.mfnav', '[data-mfnav="mobileBack"]', onClickBack);

            mobileNav
                .find(navSelector)
                .removeClass('nav')
                .removeClass('navbar-nav');

            mobileNav
                .find(subNavSelector)
                .addClass('sub-nav')
                .hide();

            var showBreadcrumbs = $(mobileNav).data("show-breadcrumbs");
            if (showBreadcrumbs == false) {
                $(crumbsSelector).hide();
            }

        }
        function activate($root) {
            $root.data('mfnav-mode', 'mobile');
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
        }


        function onClickLink(event) {
            var $navItem = $(event.currentTarget).closest(navItemSelector),
                $subNav = $navItem.children(subNavSelector);

            if ($subNav.length) {
                event.preventDefault();
                showSubNav($subNav);
                return;
            }

            if (tryLoadSubNav($navItem)) {
                event.preventDefault();
            }
        }

        function onClickBack(event) {
            event.preventDefault();

            var mobileNav = $(event.delegateTarget),
                $subNav = mobileNav.data('mfnav-route').pop(),
                $crumbs = mobileNav.find(crumbsSelector),
                $parentNav = $subNav.parent().closest(subNavSelector);

            $crumbs.children().last().remove();

            updateHeader(mobileNav, $parentNav);

            $subNav.animate({ left: "100%" }).promise().done(function () {
                $subNav.hide();

                mobileNav
                    .find(navSelector)
                    .height($parentNav.length ? $parentNav.height() : '100%');
            });
        }

        function addCrumb($sidr, $subNav) {
            var $crumbs = $sidr.find(crumbsSelector),
                $link = $subNav.siblings('A').clone();

            $link.find('.glyphicon').remove();
            $crumbs.append($('<div>').html($link));
        }

        function showSubNav($subNav) {
            var $nav = $subNav.closest(navSelector),
                $mobileNav = $nav.closest(mobileNavSelector);

            $mobileNav.data('mfnav-route').push($subNav);

            $nav.height($subNav.height());
            $subNav.show().animate({ left: 0 });

            addCrumb($mobileNav, $subNav);
            updateHeader($mobileNav, $subNav);
        }

        function updateHeader($sidr, $subNav) {
            var $header = $sidr.find(headerSelector),
                $headerLink;

            if (!$subNav.length) {
                $header.hide();
                return;
            }

            $headerLink = $subNav.siblings('A').clone();
            $headerLink.find('span').remove();

            $header
                .show()
                .find('[data-mfnav="mobileTitle"]')
                .html($headerLink);
        }

        function tryLoadSubNav($navItem) {
            var level,
                maxLevel;

            if ($navItem.data('mfnav-has-sub-nav')) {
                level = ($navItem.parent().data('mfnav-level') || 1);
                maxLevel = $navItem.closest(navSelector).data('mfnav-max-levels');

                if (level < maxLevel) {
                    common.loadSubNav($navItem, onSubNavLoaded);
                    return true;
                }
            }

            return false;
        }

        function onSubNavLoaded($subNav) {
            showSubNav($subNav);
        }

        return {
            init: init,
            activate: activate,
            deactivate: deactivate
        };
    }());
}());

$(document).ready(function () {

    var leftMenuToggle = '#left-menu-toggle';
    var slideActive = 'show-menu-left';
    var togglerRight = '#right-menu-toggle';
    var slideActiveRight = 'show-menu-right';



    $(leftMenuToggle).on("click", function (e) {
        e.preventDefault();
        $("html").removeClass(slideActiveRight);
        $("html").toggleClass(slideActive);

        var selected = $("html").hasClass(slideActive);
        if (selected) {
            $("#mfnav-mobile").appendTo($(".left-menu"));
        }

    });

    $(togglerRight).on("click", function (e) {
        e.preventDefault();

        $("html").removeClass(slideActive);
        $("html").toggleClass(slideActiveRight);
        var selected = $("html").hasClass(slideActiveRight);
    });

    $(document).on("mouseup", (function (e) {
        var container = $(".site-overlay");
        var close = $(".menu-close");
        if (container.is(e.target) || close.is(e.target)) {
            e.preventDefault();
            $("html").removeClass(slideActive);
            $("html").removeClass(slideActiveRight);
        }
    }));

    $(window).on("resize", function () {
        if ($(window).width() > 767 && $('.navbar-toggle').is(':hidden')) {
            $("html").removeClass(slideActive);
            $("html").removeClass(slideActiveRight);
        }
    });

});