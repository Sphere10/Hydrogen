window.toggleModal = () => {
    $("#modal").modal('toggle');
}

window.initializeModal = (reference) => {
    $('#modal').on('hidden.bs.modal', function (e) {
        reference.invokeMethod('OnModalClosed');
        $('#modal').modal('hide');
    })
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