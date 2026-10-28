/**
 * save user.
 **/
function saveUser() {
    try {

        disableUserSaveButton();
        resetErrorMessagesDiv();

        let id = $('#hfRecordId').val();
        let firstName = $('#user-first-name').val();
        let lastName = $('#user-last-name').val();
        let displayName = $('#user-display-name').val();
        let email = $('#user-email').val();
        let verifyEmail = $('#user-verify-email').val();
        let isActive = $('#user-isactive').prop('checked');
        let classIds = $('#user-teacher-classes').val().toString();

        // reset
        $('.form-user').find('.form-control').each(function () {
            $(this).removeClass('is-invalid');
        });

        // client side validations
        let re = /^[\w\s-./]+$/;

        var teacherClassesCount = $('#user-teacher-classes > option').length;
        if (teacherClassesCount === 0) {
            $('#divErrorMessage').html("Please add student classes to proceed");
            $('#divErrorMessagesRow').removeClass('d-none');
            return;
        }
        
        // first name
        if (firstName.length === 0 || firstName.length > 50 || !re.exec(firstName)) {
            $('#user-first-name').addClass('is-invalid');
            $('#user-first-name').focus();
            return;
        }

        // last name
        if (lastName.length === 0 || lastName.length > 50 || !re.exec(lastName)) {
            $('#user-last-name').addClass('is-invalid');
            $('#user-last-name').focus();
            return;
        }

        // display name
        if (displayName.length === 0 || displayName.length > 100 || !re.exec(displayName)) {
            $('#user-display-name').addClass('is-invalid');
            $('#user-display-name').focus();
            return;
        }

        // email 
        let reEmail = /^[a-z0-9._%+-]{2,}@[a-z0-9.-]{2,}\.[a-z]{2,3}$/;
        if (email.length === 0 || email.length > 256 || !reEmail.exec(email)) {
            $('#user-email').addClass('is-invalid');
            $('#user-email').focus();
            return;
        }

        // class Ids
        let regexClassIds = /^[0-9,]+$/;
        if (classIds.length === 0 || !regexClassIds.exec(classIds)) {
            $('button.multiselect').addClass(['btn', 'btn-danger']);
            $('button.multiselect').removeClass('custom-select');
            $('button.multiselect').focus();
            return;
        }

        // save user
        if (id > 0) {
            let putrequest = {
                "firstName": firstName,
                "lastName": lastName,
                "displayName": displayName,
                "userId": id,
                "isActive": isActive,
                "classIds": classIds
            };

            $.ajax({                
                data: putrequest,
                dataType: 'json',
                type: "PUT",
                url: '/api/u/organizationuser',
                success: function (d) {
                    enableUserSaveButton();
                    // close modal
                    $('#addUserModal').modal('hide');

                    // show success message
                    $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                    $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                    $('#statusModal .modal-header').addClass('bg-primary');
                    $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                    $('#statusModal .modal-body').html('User details saved successfully.');
                    $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });

                    // re-load users
                    loadUsers();
                },
                error: function (e) {
                    enableUserSaveButton();

                    if (e.status === 400) {
                        $('#divErrorMessage').html(e.responseText); // Message: Maximum users limit reached.
                        $('#divErrorMessagesRow').removeClass('d-none');
                    }
                    else if (e.status === 409) {
                        $('#divErrorMessage').html(e.responseText); // Message: Email was already registered.
                        $('#divErrorMessagesRow').removeClass('d-none');
                    }
                    else
                        console.log(e);
                },
            });
        }
        else {
            let request = {
                "firstName": firstName,
                "lastName": lastName,
                "displayName": displayName,
                "email": email,
                "emailConfirmed": !verifyEmail,
                "classIds": classIds
            };           

            $.post('/api/u/organizationuser', request, 'json')
                .done(function (data) {
                    enableUserSaveButton();
                    // close modal
                    $('#addUserModal').modal('hide');

                    // show success message
                    $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                    $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                    $('#statusModal .modal-header').addClass('bg-primary');
                    $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                    $('#statusModal .modal-body').html('User details saved successfully.');
                    $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });

                    // re-load users
                    loadUsers();
                })
                .fail(function (e) {

                    enableUserSaveButton();

                    if (e.status === 400) {
                        $('#divErrorMessage').html(e.responseText); // Message: Maximum users limit reached.
                        $('#divErrorMessagesRow').removeClass('d-none');
                    }
                    else if (e.status === 409) {
                        $('#divErrorMessage').html(e.responseText); // Message: Email was already registered.
                        $('#divErrorMessagesRow').removeClass('d-none');
                    }
                    else
                        console.log(e);
                });
        }

    } catch (e) {
        console.log(e);
    }
    finally {
        enableUserSaveButton();
    }
}

/**
 * reset user password
 **/
function resetUserPassword() {
    try {
                
        $('#divResetPasswordErrorMessagesRow').addClass('d-none');
        $('#divResetPasswordErrorMessage').html('');

        let emailId = $('#hfEmailId').val();        

        $.get(`/api/u/organizationuser/resetpassword?emailId=${emailId}`)
            .done(function (data) {
                $('#addUserResetModal').modal('hide');               
                // show success message
                $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                $('#statusModal .modal-header').addClass('bg-primary');
                $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                $('#statusModal .modal-body').html(`Password is successfully reset to ${data}.`);
                $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });
            }).fail(function (e) {
                console.log(e);
                $('#divResetPasswordErrorMessage').html(e.responseText);
                $('#divResetPasswordErrorMessagesRow').removeClass('d-none');
            });
       

    } catch (e) {
        console.log(e);
    }
    finally {
        enableUserSaveButton();
    }
}

function disableUserSaveButton() {
    $('.btn-save-user').prop('disabled', true);
    $('.btn-save-user').prop('innerHTML', 'Saving...');
}

function enableUserSaveButton() {
    $('.btn-save-user').prop('disabled', false);
    $('.btn-save-user').prop('innerHTML', 'Save');
}

function resetErrorMessagesDiv() {
    $('#divErrorMessagesRow').addClass('d-none');
    $('#divErrorMessage').html('');
}

/**
 * load users.
 **/
function loadUsers() {
    try {
        $.get('/api/u/organizationuser/all')
            .done(function (data) {
                let users = data;
                let rows = '';
                if (users && users.length > 0) {
                    $('.table-users > tbody').html('');

                    for (var i = 0; i < users.length; i++) {
                        let _item = users[i];
                        let row = document.createElement('tr');
                        row.setAttribute('class', 'bg-white');
                        // first name
                        let cell0 = row.insertCell(0);
                        cell0.innerHTML = `${_item.firstName}`;
                        // last name
                        let cell1 = row.insertCell(1);
                        cell1.innerHTML = `${_item.lastName}`;
                        // display name
                        let cell2 = row.insertCell(2);
                        cell2.innerHTML = `${_item.displayName}`;
                        // email
                        let cell3 = row.insertCell(3);
                        cell3.innerHTML = `${_item.email}`; 

                        // IsActive
                        let cell4 = row.insertCell(4);
                        cell4.innerHTML = `${_item.isActive ? "Yes" : "No"}`; 
                        
                        // edit
                        let cell5 = row.insertCell(5);
                        cell5.setAttribute('class', 'text-center');
                        cell5.innerHTML = `<img class="img-user-edit cursor-pointer" src="/images/edit.svg" width="24" height="24" data-id='${_item.userId}' data-toggle="tooltip" data-placement="top" title="Edit User" alt="Edit" />`;
                        $('.table-users > tbody').append(row);  

                        // reset password
                        let cell6 = row.insertCell(6);
                        cell6.setAttribute('class', 'text-center');
                        cell6.innerHTML = `<img class="img-user-reset cursor-pointer" src="/images/edit.svg" width="24" height="24" data-id='${_item.userId}' data-toggle="tooltip" data-placement="top" title="Reset Password" alt="Reset Password" />`;
                        $('.table-users > tbody').append(row);  
                    }                    

                    // wire-up click event for 'Edit'
                    $('.table-users > tbody').on('click', 'img.img-user-edit', function (e) {
                        editUser(e);
                    });

                    // wire-up click event for 'Reset Password'
                    $('.table-users > tbody').on('click', 'img.img-user-reset', function (e) {
                        openResetUserPasswordModel(e);
                    });

                    // show tooltip
                    $('[data-toggle="tooltip"]').tooltip();
                }
            })
            .fail(function (error) {
                console.log(error);
            });
    } catch (e) {
        console.log(e);
    }
}

/**
 * edit user.
 * @param {any} e
 */
function editUser(e) {
    try {
        let id = $(e.currentTarget).data('id');
        $('#hfRecordId').val(id);
        $('.row-verify-email').hide();        
        $.get(`/api/u/organizationuser/${id}`)
            .done(function (data) {
                let user = data;
                if (user) {
                    resetUserForm();
                    $('#user-first-name').val(user.firstName);
                    $('#user-last-name').val(user.lastName);
                    $('#user-email').val(user.email);
                    $('#user-display-name').val(user.displayName);
                    $('#user-isactive').prop("checked", user.isActive);
                    $('#user-teacher-classes').multiselect('select', user.classIds.split(','));
                }
                $('#user-email').attr('disabled', 'disabled');
                $('#addUserModalLabel').html('Edit User');
                $('#addUserModal').modal({ backdrop: 'static', keyboard: false, show: true });
            })
            .fail(function (e) {
                console.log(e);
                $('#addUserModal').modal('hide');
            });
    } catch (e) {
        console.log(e);
    }
}

function openResetUserPasswordModel(e) {
    try {
        let id = $(e.currentTarget).data('id');
        $('#hfRecordId').val(id);
        $('#hfEmailId').val('');
        $('#divResetPasswordErrorMessage').html('');
        $.get(`/api/u/organizationuser/${id}`)
            .done(function (data) {
                let user = data;
                if (user) {
                    $('#hfEmailId').val(data.email);
                }
                $('#addUserResetModalLabel').html('Reset User Password');
                $('#addUserResetModal').modal({ backdrop: 'static', keyboard: false, show: true });
            })
            .fail(function (e) {
                console.log(e);
                $('#hfEmailId').val('');
                $('#addUserResetModal').modal('hide');
            });
        
    } catch (e) {
        console.log(e);
    }
}

/**
 * reset user form.
 **/
function resetUserForm() {
    enableUserSaveButton();
    // reset form controls css classes
    $('.form-user').find('.form-control').each(function () {
        $(this).removeClass('is-invalid');
    });

    // first name
    $('#user-first-name').val('');

    // last name
    $('#user-last-name').val('');

    // display name
    $('#user-display-name').val('');

    // email
    $('#user-email').val(''); 

    // verify email
    $("#user-verify-email").val(false);

    // remove disabled attribute
    $('#user-email').removeAttr('disabled');

    //
    // teacher classes
    $('#user-teacher-classes').multiselect('deselectAll', false);
    $('button.multiselect').addClass('custom-select');
    $('button.multiselect').removeClass(['btn', 'btn-danger']);

    resetErrorMessagesDiv();
}

$("document").ready(function () {

    $('#user-teacher-classes').multiselect({
        maxHeight: 200,
        nonSelectedText: 'Select classes',
        numberDisplayed: 4,
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        onDropdownHidden: function (e) {
            let teacherClasses = $('#user-teacher-classes').val().toString();

            if (teacherClasses.length === 0) {
                $('button.multiselect').addClass(['btn', 'btn-danger']);
                $('button.multiselect').removeClass('custom-select');
                $('button.multiselect').focus();
                return;
            } else {
                $('button.multiselect').addClass('custom-select');
                $('button.multiselect').removeClass(['btn', 'btn-danger']);
            }
        }
    });
    
    // tooltip
    $('[data-toggle="tooltip"]').tooltip();

    // add users
    $('.table-users').on('click', '.img-add-user', function () {
        resetUserForm();
        $('#addUserModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    // load users
    loadUsers();

    $('.btn-add-user').click(function () {
        resetUserForm();
        $('#hfRecordId').val('0');
        $('.row-verify-email').show();
        $('#addUserModalLabel').html('Add User');
        $('#addUserModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    $('#addUserModal').on('hidden.bs.modal', function (e) { $('#hfRecordId').val('0'); });

    var lastActionTime, difference;
    $('.btn-save-user').click(function (e) {
        if (lastActionTime) {
            difference = e.timeStamp - lastActionTime;
            if (difference >= 1500) { // 1500: Milliseconds              
                saveUser();
            }
        }
        else {
            lastActionTime = e.timeStamp;
            saveUser();
        }
        lastActionTime = e.timeStamp;
    });

    $('.btn-save-user').dblclick(function (e) {
        e.preventDefault();
    });

    $('.btn-reset-user-password').click(function (e) {
        resetUserPassword();
    });

    $('.btn-reset-user-password').dblclick(function (e) {
        e.preventDefault();
    });

    // validation: form elements
    

    // first name
    $('#user-first-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        if (e.target.value.length === 0 || e.target.value.length > 50 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    $('#user-last-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        if (e.target.value.length === 0 || e.target.value.length > 50 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    $('#user-display-name').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        if (e.target.value.length === 0 || e.target.value.length > 100 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // email
    $('#user-email').on('change', function (e) {
        let reEmail = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
        $(this).removeClass('is-invalid');
        if (e.target.value.length === 0 || e.target.value.length > 256 || !reEmail.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    }); 
});