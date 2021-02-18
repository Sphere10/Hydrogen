window.dispatchContentLoadedEvent = () => window.document.dispatchEvent(new Event("DOMContentLoaded", {
    bubbles: true,
    cancelable: true
}));

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