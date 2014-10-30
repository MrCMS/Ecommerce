(function ($) {
    var cookiesEnabled = function () {
        var cookieEnabled = (navigator.cookieEnabled) ? true : false;

        if (typeof navigator.cookieEnabled == "undefined" && !cookieEnabled) {
            document.cookie = "testcookie";
            cookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false;
        }
        return (cookieEnabled);
    }
    var analytics = function () {
        var userGuidKey = 'mrcms.analytics.user';
        var userSessionKey = 'mrcms.analytics.session';
        function logPageView() {
            if (!cookiesEnabled() || !$.cookie)
                return;
            var user = $.cookie(userGuidKey);
            if (!user)
                return;
            var session = $.cookie(userSessionKey);
            if (!session)
                return;
            var url = location.href;
            var data = {
                user: user,
                session: session,
                url: url
            }
            $.post('/analytics/log-page-view', data);
        }

        return {
            logPageView: logPageView
        };
    };

    $(function () {
        analytics().logPageView();
    });
})(jQuery);