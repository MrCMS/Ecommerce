$(function () {
    $("#add-category").bind("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
		    $(this).data("autocomplete").menu.active) {
            event.preventDefault();
        }
    }).autocomplete({
        source: function (request, response) {
            var ids = $('ul#category-list > li > input').map(function(index, element) {
                return parseInt($(element).val());
            }).get();
            $.ajax({
                url: "/Admin/Apps/Ecommerce/Category/Search",
                type: "POST",
                data: {
                    term: request.term,
                    ids: ids
                },
                traditional: true,
                success: response
            });
        },
        search: function () {
            // custom minLength
            var term = this.value;
            if (term.length < 2) {
                return false;
            }
        },
        focus: function () {
            // prevent value inserted on focus
            return false;
        },
        select: function (event, ui) {
            var i = $('ul#category-list li').length;
            $('<li>')
                .append($('<input>').attr('type', 'hidden').attr('id', 'Categories_' + i + '__Id').attr('name', 'Categories[' + i + '].Id').attr('value', ui.item.id))
                .append(ui.item.value)
                .append($('<a>').attr('href', '#').addClass('remove-category').append('x'))
                .appendTo($('ul#category-list'));
            this.value = '';
            return false;
        }
    });
    $(document).on('click', '.remove-category', function () {
        console.log($(this).data('index'));
        var ul = $(this).parents('ul');
        $(this).parents('li').remove();
        ul.find('li').each(function (index, element) {
            $(element).find('input').attr('name', 'Categories[' + index + '].Id');
        });
        return false;
    });
});