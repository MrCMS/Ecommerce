(function() {
    'use strict';

    var common,
        mobile,
        desktop,
        rootSelector = '[data-mfnav="root"]',
        navSelector = '[data-mfnav="nav"]',
        subNavSelector = '[data-mfnav="subNav"]',
        navItemSelector = '[data-mfnav="item"]';

    (function() {
        $(function() {
            var $root = $(rootSelector);

            mobile.init($root);
            desktop.init($root);
            setMode($root);

            $(window).resize(function() {
                setMode($root);
            });
        });

        function setMode($root) {
            var isReponsiveLayoutDesktop = $root.css('width') !== "767px";

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

    common = (function() {
        function loadSubNav($navItem, callback) {
            var url = $navItem.closest(navSelector).data('mfnav-url'),
                data = { parentId: $navItem.data('mfnav-id') };

            $.get(url, data, function(response) {
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

            $.each(data, function() {
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

    desktop = (function() {
        function init($root) {
            var $nav = $root.children(navSelector);

            $nav
                .on('mouseenter.mfnav', navItemSelector, onMouseEnter)
                .on('mouseleave.mfnav', navItemSelector, onMouseLeave);

            $nav
                .find(subNavSelector)
                .addClass('dropdown-menu');
        }

        function activate($root) {
            $root.data('mfnav-mode', 'desktop');
            resize($root);
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
        }

        function resize($root) {
            var $nav = $root.children(navSelector);

            $nav.children(navItemSelector).has(subNavSelector).each(function() {
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

        function onMouseEnter(event) {
            var $navItem = $(event.currentTarget),
                $subNav = $navItem.children(subNavSelector);

            if ($subNav.length) {
                showSubNav($subNav);
                return;
            }

            tryLoadSubNav($navItem);
        }

        function onMouseLeave(event) {
            var $navItem = $(event.currentTarget),
                $subNav = $navItem.children(subNavSelector);

            if ($subNav.length) {
                hideSubNav($subNav);
            }
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

    mobile = (function() {
        var sidrSelector = '#mfnav-mobile',
            headerSelector = '[data-mfnav="mobileHeader"]',
            crumbsSelector = '[data-mfnav="mobileCrumbs"]';

        function init() {
            $().sidr({
                name: sidrSelector.replace('#', ''),
                source: '[data-mfnav="mobile"], ' + rootSelector,
                renaming: false
            });

            var $sidr = $(sidrSelector);

            $sidr
                .data('mfnav-route', [])
                .on('click.mfnav', navItemSelector + ' > A', onClickLink)
                .on('click.mfnav', '[data-mfnav="mobileBack"]', onClickBack);

            $sidr
                .find(navSelector)
                .removeClass('nav')
                .removeClass('navbar-nav');

            $sidr
                .find(subNavSelector)
                .addClass('sub-nav')
                .hide();

            $(document)
                .on('click.mfnav', '[data-mfnav="toggleMobileNav"]', onClicktoggleMobileNav);
        }

        function activate($root) {
            $root.data('mfnav-mode', 'mobile');
        }

        function deactivate($root) {
            $root.data('mfnav-mode', '');
            $.sidr('close', sidrSelector.replace('#', ''));
        }

        function onClicktoggleMobileNav(event) {
            event.preventDefault();
            $.sidr('toggle', sidrSelector.replace('#', ''));

            var className = $("#sidebarmenu").attr("class");
            var adminBarOn = false;
            if ($(".mrcms-admin-nav-bar").length > 0) {
                adminBarOn = true;
            }
            
            if (className.indexOf("o") >= 0) {

                $("#sidebarmenu").removeClass().addClass("visible-xs visible-sm c");
                if (adminBarOn) {
                    $("#fixedHeader").attr("style", "left:0;top:30px;");
                    $("#search").attr("style", "left:0;top:69px;");
                } else {
                    $("#fixedHeader").attr("style", "left:0;top:0;");
                    $("#search").attr("style", "left:0;top:39px;");
                }
            }
            else if (className.indexOf("c") >= 0) {

                $("#sidebarmenu").removeClass().addClass("visible-xs visible-sm o");
                if (adminBarOn) {
                    $("#mfnav-mobile").attr("style", "top:30px;display: block; left: 0px;");
                    $("#fixedHeader").attr("style", "left:260px;top:30px;width:100%;");
                    $("#search").attr("style", "left:260px;top:69px;width:100%;");
                } else {
                    $("#fixedHeader").attr("style", "left:260px;top:0;overflow-x:hidden;width:100%;");
                    $("#search").attr("style", "left:260px;top:39px;width:100%;");
                }

            }
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

            var $sidr = $(event.delegateTarget),
                $crumbs = $sidr.find(crumbsSelector),
                $subNav = $sidr.data('mfnav-route').pop(),
                $parentNav = $subNav.parent().closest(subNavSelector);

            $crumbs.children().last().remove();
            updateHeader($sidr, $parentNav);

            $subNav.animate({ left: 260 }).promise().done(function() {
                $subNav.hide();

                $sidr
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
                $sidr = $nav.closest(sidrSelector);

            $sidr.data('mfnav-route').push($subNav);

            $nav.height($subNav.height());
            $subNav.show().animate({ left: 0 });

            addCrumb($sidr, $subNav);
            updateHeader($sidr, $subNav);
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