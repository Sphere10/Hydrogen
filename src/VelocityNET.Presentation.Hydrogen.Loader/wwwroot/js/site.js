window.showModal = () => {
    $("#modal").modal({
        backdrop: "static",
        keyboard: false
    });
}

window.hideModal = () => {
    $("#modal").modal('hide')
}

window.addDropdownHover = () => {
    $('.hover-dropdown').hover(function () {
            $('.hover-dropdown > .dropdown-menu').addClass('show');
        },
        function () {
            $(this).removeClass('show');
            $('.hover-dropdown > .dropdown-menu').removeClass('show');
        });
}

window.initializeToolTips = () => {
    $('[data-toggle="tooltip"]').tooltip()
}

window.initializeSearchDropdowns = () => {
    var input = $('.search-input');
    
    input.keyup(() => {
        if (input.val().length === 0)
        {
            $('.search-input-results').removeClass('show');
        }
        else 
        {
            $('.search-input-results').addClass('show');
        }
    });

    input.blur(() => {
        $('.search-input-results').removeClass('show');
    });
}