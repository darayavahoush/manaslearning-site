/**
 * load profile 
 **/
function loadProfile() {
    $.get('/api/u/profile', function () { })
        .done(function (d) {
            let profile = d;
            $('#first-name').val(profile.firstName);
            $('#last-name').val(profile.lastName);
            $('#display-name').val(profile.displayName);
            $('#email-address').val(profile.email);
            $('#phone-number').val(profile.phone);

            if (parseInt(profile.loginType) === 1) {
                $('#radio-parent').removeAttr('checked');
                $('#radio-teacher').attr('checked', 'checked');
            }
            else if (parseInt(profile.loginType) === 2) {
                $('#radio-teacher').removeAttr('checked');
                $('#radio-parent').attr('checked', 'checked');
            }
            else {
                $('#radio-teacher').removeAttr('checked');
                $('#radio-parent').removeAttr('checked');
            }
        })
        .fail(function (f) {
            console.log('fail');
        })
        .always(function () {
            // do nothing
        });
}

/**
 * update profile
 **/
function updateProfile() {
    try {
        let firstName = $('#first-name').val();
        let lastName = $('#last-name').val();
        let displayName = $('#display-name').val();
        let phoneNumber = $('#phone-number').val();

        // validations
        let re = /^[\w\s-./]+$/;
        $('.form-general').find('.form-control').each(function () {
            $(this).removeClass('is-invalid');
        });
        // first name
        var ok = re.exec(firstName);
        if (!ok) {
            $('#first-name').addClass('is-invalid');
            $('#first-name').focus();
            return;
        }
        else if (firstName.length > 50) {
            $('#first-name').addClass('is-invalid');
            $('#first-name').focus();
            return;
        }

        // last name
        ok = re.exec(lastName);
        if (!ok) {
            $('#last-name').addClass('is-invalid');
            $('#last-name').focus();
            return;
        }
        else if (lastName.length > 50) {
            $('#last-name').addClass('is-invalid');
            $('#last-name').focus();
            return;
        }

        // display name
        ok = re.exec(displayName);
        if (!ok) {
            $('#display-name').addClass('is-invalid');
            $('#display-name').focus();
            return;
        }
        else if (displayName.length > 100) {
            $('#display-name').addClass('is-invalid');
            $('#display-name').focus();
            return;
        }

        // phone number
        ok = re.exec(phoneNumber);
        if (!ok) {
            $('#phone-number').addClass('is-invalid');
            $('#phone-number').focus();
            return;
        }
        else if (phoneNumber.length !== 10) {
            $('#phone-number').addClass('is-invalid');
            $('#phone-number').focus();
            return;
        }
        else if (isNaN(phoneNumber)) {
            $('#phone-number').addClass('is-invalid');
            $('#phone-number').focus();
        }

        let request = {
            firstName: firstName,
            lastName: lastName,
            displayName: displayName,
            phoneNumber: phoneNumber
        };

        // update profile
        $.ajax({
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(request),
            dataType: 'json',
            type: "POST",
            url: '/api/u/profile',
            success: function (d) {
                let profile = d;
                $('#first-name').val(profile.firstName);
                $('#last-name').val(profile.lastName);
                $('#display-name').val(profile.displayName);
                $('#email-address').val(profile.email);
                $('#phone-number').val(profile.phone);

                if (parseInt(profile.loginType) === 1) {
                    $('#radio-parent').removeAttr('checked');
                    $('#radio-teacher').attr('checked', 'checked');
                }
                else if (parseInt(profile.loginType) === 2) {
                    $('#radio-teacher').removeAttr('checked');
                    $('#radio-parent').attr('checked', 'checked');
                }
                else {
                    $('#radio-teacher').removeAttr('checked');
                    $('#radio-parent').removeAttr('checked');
                }
                //$('.toast-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                //$('.toast-header').removeClass('text-white').removeClass('text-dark');

                //$('.toast-header').addClass('bg-primary').addClass('text-white');
                //$('.toast-header > strong').html('Profile');
                //$('.toast-body').html('Profile updated successfully.');
                //$('.toast').toast('show');

                $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                $('#statusModal .modal-header').addClass('bg-primary');
                $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                $('#statusModal .modal-body').html('Profile updated successfully.');
                $('#statusModal').modal({
                    keyboard: false,
                    show: true
                });
            },
            error: function (e) {
                console.log(e);
            },
        });

    } catch (e) {
        console.log(e);
    }
}

$('document').ready(function () {
    loadProfile();
    $('.btn-save').click(updateProfile);
    $('#first-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        var ok = re.exec(e.target.value);
        if (!ok) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else if (e.target.value.length > 50) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
    $('#last-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        var ok = re.exec(e.target.value);
        if (!ok) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else if (e.target.value.length > 50) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
    $('#display-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        var ok = re.exec(e.target.value);
        if (!ok) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else if (e.target.value.length > 100) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
    $('#phone-number').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        var ok = re.exec(e.target.value);
        if (!ok) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else if (e.target.value.length !== 10) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else if (isNaN(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
});