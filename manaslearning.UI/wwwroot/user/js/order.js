$(document).ready(function () {

    $('.send-invoice-email').on('click', function (eventData) {
        var id = $(this).data('bind');
        $("#divMessage").html('');
        var sucecss = `
<div class="alert alert-success alert-dismissible">
  <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
  <strong>Success!</strong> Invoice has been sent to registered email.
</div>
`;
        var error = `
<div class="alert alert-warning alert-dismissible">
<a href="#" class="closed" data-dismiss="alert" aria-lable="close">&times;</a>
<strong>Warning!</strong> Unable to sent invoice email to register email.
</div>
`;
        $.post('/api/u/Order/SendInvoiceEmail?oid=' + id)
            .done(function (data) {
                if (data == 'success')
                    $("#divMessage").html(sucecss);
                else if (data == 'BadRequest') {
                    $("#divMessage").html(error);
                }
            })
            .fail(function (e) {
                $("#divMessage").html(error);
            });
    });
});