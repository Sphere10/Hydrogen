window.toggleModal = () => {
    $("#modal").modal('toggle');
}

window.initializeModal = (reference) =>
{
    $('#modal').on('hidden.bs.modal', function (e) {
        reference.invokeMethod('OnModalClosed');
        $('#modal').modal('hide');
    })
}