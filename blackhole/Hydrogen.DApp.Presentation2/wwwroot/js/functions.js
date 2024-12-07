window.dispatchContentLoadedEvent = () => window.document.dispatchEvent(new Event("DOMContentLoaded", {
    bubbles: true,
    cancelable: true
}));

window.showModal = () => {
    $("#modal").modal({
        backdrop: "static",
        keyboard: false
    });
}

window.hideModal = () => {
    $("#modal").modal('hide')
}

window.initDataTableById = (id, options) => {
    $('#' + id).DataTable(options);
}

window.clipboardCopy = {
    copyText: function(text) {
        navigator.clipboard.writeText(text)
            .catch(function (error) {
                alert(error);
            });
    }
};

document.addEventListener("DOMContentLoaded", () => {
    let input = $('.search-input');

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
});