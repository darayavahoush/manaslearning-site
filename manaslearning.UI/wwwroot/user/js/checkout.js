$('document').ready(function () {
    let hfIsAddressAvailable = $('#hfIsAddressAvailable').val();
    if (hfIsAddressAvailable === 'false') {
        //$('#addressModal').modal({ backdrop: 'static', keyboard: false, show: true });
    }
    else if (hfIsAddressAvailable === 'true') {
        // do nothing
    }

    $('#addressForm .it-na').change(address.validation.isNameValid);
    $('#addressForm .it-al1').change(address.validation.isAddressLine1Valid);
    $('#addressForm .it-al2').change(address.validation.isAddressLine2Valid);
    $('#addressForm .it-al3').change(address.validation.isAddressLine3Valid);
    $('#addressForm .it-ci').change(address.validation.isCityValid);
    $('#addressForm .it-st').change(address.validation.isStateValid);
    $('#addressForm .it-zc').change(address.validation.isPostalCodeValid);
    $('#addressForm .it-co').change(address.validation.isCountryValid);

    $('.d-placeorder').on('click', '.btn-place-order', function () {
        if (hfIsAddressAvailable === 'false') {
            $('.btn-place-order').attr('disabled', 'disabled');
            $('.btn-place-order').attr('type', 'button');
            return;
        }
    });

    $('.d-address').on('click', '.btn-ae-address', function () {
        try {
            let id = $('#Param1').val();
            if ($.trim(id).length > 0) {
                // get existing address
                $.get(`/api/u/address?Oid=${id}`)
                    .done(function (d) {
                        if (d.length === 0)
                            return;

                        let addr = d[0];
                        address.reset();
                        $('#addressForm .it-na').val(addr.name);
                        $('#addressForm .it-al1').val(addr.addressLine1);
                        $('#addressForm .it-al2').val(addr.addressLine2);
                        $('#addressForm .it-al3').val(addr.addressLine3);
                        $('#addressForm .it-ci').val(addr.city);
                        $('#addressForm .it-st').val(addr.state);
                        $('#addressForm .it-co').val(addr.country);
                        $('#addressForm .it-zc').val(addr.zipCode);

                        $('#addressModal').modal({ backdrop: 'static', keyboard: false, show: true });
                    })
                    .fail(function (e) {
                        $('#addressModal').modal('hide');
                        console.log(e);
                    });
            }
            else {
                address.reset();
                $('#addressModal').modal({ backdrop: 'static', keyboard: false, show: true });
            }
        } catch (e) {
            // catch log
        }
    });
    $('#addressModal').on('click', '.btn-s-address', function () {
        // reset form
        address.reset();

        // client side validations
        let isAddressValid = address.validation.isNameValid()
            && address.validation.isAddressLine1Valid()
            && address.validation.isAddressLine2Valid()
            && address.validation.isAddressLine3Valid()
            && address.validation.isCityValid()
            && address.validation.isStateValid()
            && address.validation.isPostalCodeValid()
            && address.validation.isCountryValid();

        if (!isAddressValid)
            return;

        let id = $('#Param1').val();

        let request = {
            'oid': id,
            'name': $.trim($('#addressForm .it-na').val()),
            'addressLine1': $.trim($('#addressForm .it-al1').val()),
            'addressLine2': $.trim($('#addressForm .it-al2').val()),
            'addressLine3': $.trim($('#addressForm .it-al3').val()),
            'city': $.trim($('#addressForm .it-ci').val()),
            'state': $.trim($('#addressForm .it-st').val()),
            'country': $.trim($('#addressForm .it-co').val()),
            'zipCode': $.trim($('#addressForm .it-zc').val())
        }

        if ($.trim(id).length === 0) {
            $.post('/api/u/address', request, 'json')
                .done(function (d) {
                    $('#Param1').val(d.oid);
                    if ($('.d-address .list-no-address').length === 1) {
                        $('.d-address .list-no-address').parent().remove();
                        let _div = $('div.d-address div')[0];
                        // find p
                        let _p = $(_div).find('p.mt-3.mb-0')[0];
                        $(_p).remove(); // remove button
                        $(_div).append('<div class="row"><div class="col-md-12 mt-2">Add/Edit your billable address. This will appear on the invoice for your order.</div></div>');
                        $(_div).append('<h5 class="pt-3 mb-3">Sold To</h5>');
                        $(_div).append('<div class="row"><div class="col-md-6 list-address"><ul class="list-unstyled"></ul></div></div>');
                        $(_div).append('<p class="mt-3 mb-0"><button class="btn btn-sm btn-outline-primary btn-ae-address" type="button"> Edit </button></p>');

                        address.updateUI(d, 'Address', 'Address created.');
                    }
                    else {
                        // TODO: show message to user
                    }
                })
                .fail(function (e) {
                    console.log(e);
                });
        }
        else {
            $.ajax({
                type: 'PUT',
                url: '/api/u/address',
                data: request,
                success: function (d) {
                    if ($('.d-address .list-address ul').length === 1
                        || $('.d-address .list-no-address').length === 1) {

                        let message = '';

                        if ($('.d-address .list-address ul').length === 1) {
                            message = 'Address updated.';
                        }
                        else if ($('.d-address .list-no-address').length === 1) {
                            $('.d-address .list-no-address').parent().remove();
                            let _div = $('div.d-address div');
                            $(_div).append('<div class="row"><div class="col-md-12 mt-2">Add/Edit your billable address. This will appear on the invoice for your order.</div></div>');
                            $(_div).append('<h5 class="pt-3 mb-3">Sold To</h5>');
                            $(_div).append('<div class="col-md-6 list-address"></div>');
                            message = 'Address created.';
                        }
                        address.updateUI(d, 'Address', message);
                    }
                },
                error: function (e) { console.log(e); },
                dataType: 'json'
            });
        }
    });

    //$('.it-io-qu').keyup(function (e) {
    //    checkout.calculate(e);
    //});

    $('.it-io-qu').change(function (e) {
        checkout.calculate(e);
    });
});

var address = {
    re: /^[\w\s-./]+$/,
    nameRe: /^[\w\s]+$/,
    addressLineRe: /^[\w\s-.,/]+$/,
    addressCityRe: /^[\w\s]+$/,
    addressStateRe: /^[a-z A-Z]+$/,
    addressCountryRe: /^[a-z A-Z]+$/,
    numbersRe: /^[0-9]+$/,
    validation: {
        isNameValid: function () {
            // name
            if ($('#addressForm .it-na').length === 1) {
                let e = $('#addressForm .it-na');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.re.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return;
                }
                else if (val.length > 50) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                return true;
            } else
                return false;
        },
        isAddressLine1Valid: function () {
            // address line 1
            if ($('#addressForm .it-al1').length === 1) {
                let e = $('#addressForm .it-al1');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.addressLineRe.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return false;
                }
                else if (val.length > 200) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                return true;
            } else
                return false;
        },
        isAddressLine2Valid: function () {
            // address line 2 (optional)
            if ($('#addressForm .it-al2').length === 1) {
                let e = $('#addressForm .it-al2');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length !== 0) {
                    if (!address.addressLineRe.exec(val)) {
                        address.validation.showAddressValidationMessage(e, 0);
                        return false;
                    }
                    else if (val.length > 200) {
                        address.validation.showAddressValidationMessage(e, 1);
                        return false;
                    }
                }
                return true;
            } else
                return false;
        },
        isAddressLine3Valid: function () {
            // address line 3 (optional)
            if ($('#addressForm .it-al3').length === 1) {
                let e = $('#addressForm .it-al3');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length !== 0) {
                    if (!address.addressLineRe.exec(val)) {
                        address.validation.showAddressValidationMessage(e, 0);
                        return false;
                    }
                    else if (val.length > 200) {
                        address.validation.showAddressValidationMessage(e, 1);
                        return false;
                    }
                }
                return true;
            } else
                return false;
        },
        isCityValid: function () {
            // city
            if ($('#addressForm .it-ci').length === 1) {
                let e = $('#addressForm .it-ci');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.addressCityRe.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return false;
                }
                else if (val.length > 50) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                return true;
            } else
                return false;
        },
        isStateValid: function () {
            // state
            if ($('#addressForm .it-st').length === 1) {
                let e = $('#addressForm .it-st');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.addressStateRe.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return false;
                }
                else if (val.length > 50) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                return true;
            } else
                return false;
        },
        isPostalCodeValid: function () {
            // postal code
            if ($('#addressForm .it-zc').length === 1) {
                let e = $('#addressForm .it-zc');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.numbersRe.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return false;
                }
                else if (val.length < 6) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                else if (val.length > 10) {
                    address.validation.showAddressValidationMessage(e, 3);
                    return false;
                }
                return true;
            } else
                return false;
        },
        isCountryValid: function () {
            // country
            if ($('#addressForm .it-co').length === 1) {
                let e = $('#addressForm .it-co');
                let val = $.trim($(e).val());

                // reset control errors
                address.validation.hideAddressValidationMessage(e);

                if (val.length === 0) {
                    address.validation.showAddressValidationMessage(e, 0);
                    return false;
                }
                else if (!address.addressCountryRe.exec(val)) {
                    address.validation.showAddressValidationMessage(e, 1);
                    return false;
                }
                else if (val.length > 50) {
                    address.validation.showAddressValidationMessage(e, 2);
                    return false;
                }
                return true;
            } else
                return false;
        },
        showAddressValidationMessage: function (e, i) {
            if (isNaN(i)) return;

            let index = parseInt(i);
            if (index < 0) return;

            $(e).addClass('is-invalid');
            $(e).parent().find(`.invalid-feedback span:eq(${index})`).each(function () {
                $(this).removeClass('d-none').addClass('d-block');
            });
            $(e).focus();
        },
        hideAddressValidationMessage: function (e) {
            $(e).removeClass('is-invalid');
            $(e).parent().find('.invalid-feedback span').each(function (index, element) {
                $(element).removeClass('d-block').addClass('d-none');
            });
        }
    },
    /**
     * rest address edit form
     * */
    reset: function () {
        // form reset
        $('#addressForm').find('.form-control').each(function (i, e) {
            $(e).removeClass('is-invalid');
        });
        $('#addressForm').find('.invalid-feedback span').each(function (i, e) {
            $(e).removeClass('d-block').addClass('d-none');
        });
    },
    updateUI: function (d, title, message) {
        let _ul = $('.d-address .list-address ul');
        $(_ul).html('');
        $(_ul).append($('<li />').html(d.name));
        $(_ul).append($('<li />').html(d.addressLine1));
        $(_ul).append($('<li />').html(d.addressLine2));
        $(_ul).append($('<li />').html(d.addressLine3));
        $(_ul).append($('<li />').html(d.city));
        $(_ul).append($('<li />').html(d.state));
        $(_ul).append($('<li />').html(d.country));
        $(_ul).append($('<li />').html(d.zipCode));

        $('#addressModal').modal('hide');

        $('.btn-place-order').removeAttr('disabled');
        $('.btn-place-order').attr('type', 'submit');

        address.showSuccessMessage(title, message);
    },
    showSuccessMessage: function (title, message) {
        // show success message
        $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
        $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
        $('#statusModal .modal-header').addClass('bg-primary');
        $('#statusModal .modal-title').html(`<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> ${title}`).addClass('text-white');
        $('#statusModal .modal-body').html(message);
        $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });
    }
}

var checkout = {
    calculate: function (e) {

        let p = $('#Item_ItemPrice').val();
        if (!isFinite(p)) {
            return; // TODO: show message
        }

        let g = $('#Item_GSTRate').val();
        if (!isFinite(g)) {
            return; // TODO: show message
        }

        let q = $(e.target).val();
        if (q === '' || !isFinite(q) || parseFloat(q) <= 0) {
            q = 1; // default
            $(e.target).val(q);
        }

        let price = parseFloat(p);
        let qty = parseInt(q);
        let gst = parseFloat(g);
        let subtotal = 0.00;
        let tax = 0.00;

        $(e.target).val(qty);
        $('.it-io-qu-2').html(qty);
        subtotal = price * qty;

        if (isNaN(subtotal)) {
            subtotal = 0;
            tax = 0;
        } else {
            $('.it-io-st').html(`₹ ${subtotal.toFixed(2)}`);
            $('.it-io-st-2').html(`₹ ${subtotal.toFixed(2)}`);
            tax = subtotal * (gst / 100);
        }
        $('.it-io-et').html(`₹ ${tax.toFixed(2)}`);

        let total = subtotal + tax
        $('.it-io-to').html(`₹ ${total.toFixed(2)}`);

        let hfIsAddressAvailable = $('#hfIsAddressAvailable').val();

        if (qty <= 0) {
            $('.btn-place-order').attr('disabled', 'disabled');
            $('.btn-place-order').attr('type', 'button');
            $(e.target).addClass('is-invalid');
            $(e.target).focus();
        }
        else if (hfIsAddressAvailable === 'false') {
            $('.btn-place-order').attr('disabled', 'disabled');
            $('.btn-place-order').attr('type', 'button');
        }
        else {
            $(e.target).removeClass('is-invalid');
            $('.btn-place-order').removeAttr('disabled');
            $('.btn-place-order').attr('type', 'submit');
        }
    }
}