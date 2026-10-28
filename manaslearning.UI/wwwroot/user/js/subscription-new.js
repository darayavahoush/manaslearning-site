$('document').ready(function () {
    $('#sn-param2').change(function (e) {
        if (e) {
            let q = $(e.target).val();
            if (q === '' || !isFinite(q) || parseInt(q) <= 0) {
                q = 1; // default
                $(e.target).val(q);
            }

            let qty = parseInt(q);
            $(e.target).val(qty);
            let basePrice = $(e.target).data('base-price');
            let subtotal = parseFloat(basePrice) * qty;
            $('.sub-total').html(subtotal.toFixed(2));

            $(e.target).removeClass('is-invalid');
            $('.btn-sn-buy').removeAttr('disabled');
        }
    });
    $('.btn-sn-buy').click(function () {
        let _o = $('#sn-param1').val();
        let q = $('#sn-param2').val();
        if (q !== '' && isFinite(q) && parseInt(q) > 0) {
            let qty = parseInt(q);
            window.location = `/u/purchase/checkout/${_o}/${qty}`;
        }
    });
});
