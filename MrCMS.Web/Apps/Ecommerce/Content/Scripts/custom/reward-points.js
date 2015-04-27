(function ($) {
    function toggleUseRewardPoints(event) {
        event.preventDefault();
        $.post('/Apps/Ecommerce/UseRewardPoints', { useRewardPoints: $(event.target).is(':checked') }, function(result) {
            $(document).trigger('reward-points-updated', result);
        });
    }
    $(function () {
        $(document).on('change', "[data-use-reward-points]", toggleUseRewardPoints);
    });
})(jQuery);