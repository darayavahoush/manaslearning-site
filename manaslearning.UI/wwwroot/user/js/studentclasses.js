/**
 * save student class.
 **/
function saveStudentClass() {
    try {

        disableStudentClassSaveButton();
        resetErrorMessagesDiv();

        let id = $('#hfRecordId').val();
        let className = $('#student-class-name').val();
        let classDescription = $('#student-class-description').val();        
        let isActive = $('#student-class-isactive').prop('checked');            

        // reset
        $('.form-studentclass').find('.form-control').each(function () {
            $(this).removeClass('is-invalid');
        });

        // client side validations
        let re = /^[\w\s-./]+$/;
        
        // student class name
        if (className.length === 0 || className.length > 50 || !re.exec(className)) {
            $('#student-class-name').addClass('is-invalid');
            $('#student-class-name').focus();
            return;
        }

        // student class description
        if (classDescription.length === 0 || classDescription.length > 50 || !re.exec(classDescription)) {
            $('#student-class-description').addClass('is-invalid');
            $('#student-class-description').focus();
            return;
        }

        // save student calss
        if (id.length > 0) {
            let putrequest = {
                "studentClassName": className,
                "studentClassDescription": classDescription,
                "studentClassOid": id,
                "isActive": isActive
            };

            $.ajax({                
                data: putrequest,
                dataType: 'json',
                type: "PUT",
                url: '/api/u/organizationstudentclass',
                success: function (d) {
                    enableStudentClassSaveButton();
                    // close modal
                    $('#addStudentClassModal').modal('hide');

                    // show success message
                    $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                    $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                    $('#statusModal .modal-header').addClass('bg-primary');
                    $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                    $('#statusModal .modal-body').html('Student Class details saved successfully.');
                    $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });

                    // re-load student classes
                    loadStudentClasses();
                },
                error: function (e) {
                    enableStudentClassSaveButton();

                    if (e.status === 409) {
                        $('#divErrorMessage').html(e.responseText); // Message: Student Class is already exists.
                        $('#divErrorMessagesRow').removeClass('d-none');
                    }
                    else
                        console.log(e);
                },
            });
        }
        else {
            let request = {
                "studentClassName": className,
                "studentClassDescription": classDescription
            };           

            $.post('/api/u/organizationstudentclass', request, 'json')
                .done(function (data) {
                    enableStudentClassSaveButton();
                    // close modal
                    $('#addStudentClassModal').modal('hide');

                    // show success message
                    $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                    $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                    $('#statusModal .modal-header').addClass('bg-primary');
                    $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                    $('#statusModal .modal-body').html('Student Class details saved successfully.');
                    $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });

                    // re-load student classes
                    loadStudentClasses();
                })
                .fail(function (e) {

                    enableStudentClassSaveButton();

                    if (e.status === 409) {
                        $('#divErrorMessage').html(e.responseText); // Message: Class was already registered.
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
        enableStudentClassSaveButton();
    }
}

function disableStudentClassSaveButton() {
    $('.btn-save-studentclass').prop('disabled', true);
    $('.btn-save-studentclass').prop('innerHTML', 'Saving...');
}

function enableStudentClassSaveButton() {
    $('.btn-save-studentclass').prop('disabled', false);
    $('.btn-save-studentclass').prop('innerHTML', 'Save');
}

function resetErrorMessagesDiv() {
    $('#divErrorMessagesRow').addClass('d-none');
    $('#divErrorMessage').html('');
}

/**
 * load student classes.
 **/
function loadStudentClasses() {
    try {
        $.get('/api/u/organizationstudentclass/all')
            .done(function (data) {
                let studentClasses = data;
                let rows = '';
                if (studentClasses && studentClasses.length > 0) {
                    $('.table-studentclasses > tbody').html('');

                    for (var i = 0; i < studentClasses.length; i++) {
                        let _item = studentClasses[i];
                        let row = document.createElement('tr');
                        row.setAttribute('class', 'bg-white');
                        // Class name
                        let cell0 = row.insertCell(0);
                        cell0.innerHTML = `${_item.studentClassName}`;
                        // Class description
                        let cell1 = row.insertCell(1);
                        cell1.innerHTML = `${_item.studentClassDescription}`;
                        
                        // IsActive
                        let cell2 = row.insertCell(2);
                        cell2.innerHTML = `${_item.isActive ? "Yes" : "No"}`; 
                        
                        // edit
                        let cell3 = row.insertCell(3);
                        cell3.setAttribute('class', 'text-center');
                        cell3.innerHTML = `<img class="img-studentclass-edit cursor-pointer" src="/images/edit.svg" width="24" height="24" data-id='${_item.studentClassOid}' data-toggle="tooltip" data-placement="top" title="Edit Student Class" alt="Edit" />`;
                        $('.table-studentclasses > tbody').append(row);
                        
                    }                    

                    // wire-up click event for 'Edit'
                    $('.table-studentclasses > tbody').on('click', 'img.img-studentclass-edit', function (e) {
                        editStudentClass(e);
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
 * edit student class.
 * @param {any} e
 */
function editStudentClass(e) {
    try {
        let id = $(e.currentTarget).data('id');
        $('#hfRecordId').val(id);           
        $.get(`/api/u/organizationstudentclass/${id}`)
            .done(function (data) {
                let studentClass = data;
                if (studentClass) {
                    resetStudentClassForm();
                    $('#student-class-name').val(studentClass.studentClassName);
                    $('#student-class-description').val(studentClass.studentClassDescription);
                    $("#student-class-isactive").prop("checked", studentClass.isActive);                                       
                }               
                $('#addStudentClassModalLabel').html('Edit Student Class');
                $('#addStudentClassModal').modal({ backdrop: 'static', keyboard: false, show: true });
            })
            .fail(function (e) {
                console.log(e);
                $('#addStudentClassModal').modal('hide');
            });
    } catch (e) {
        console.log(e);
    }
}

/**
 * reset student class form.
 **/
function resetStudentClassForm() {
    enableStudentClassSaveButton();
    // reset form controls css classes
    $('.form-studentclass').find('.form-control').each(function () {
        $(this).removeClass('is-invalid');
    });

    // class name
    $('#student-class-name').val('');

    // class description
    $('#student-class-description').val('');    

    resetErrorMessagesDiv();
}

$("document").ready(function () {
    
    // tooltip
    $('[data-toggle="tooltip"]').tooltip();

    // add student classes
    $('.table-studentclasses').on('click', '.img-add-studentclass', function () {
        resetStudentClassForm();
        $('#addStudentClassModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    // load student classes
    loadStudentClasses();

    $('.btn-add-studentclass').click(function () {
        resetStudentClassForm();
        $('#hfRecordId').val('');       
        $('#addStudentClassModalLabel').html('Add Student Class');
        $('#addStudentClassModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    $('#addStudentClassModal').on('hidden.bs.modal', function (e) { $('#hfRecordId').val(''); });

    var lastActionTime, difference;
    $('.btn-save-studentclass').click(function (e) {
        if (lastActionTime) {
            difference = e.timeStamp - lastActionTime;
            if (difference >= 1500) { // 1500: Milliseconds              
                saveStudentClass();
            }
        }
        else {
            lastActionTime = e.timeStamp;
            saveStudentClass();
        }
        lastActionTime = e.timeStamp;
    });

    $('.btn-save-studentclass').dblclick(function (e) {
        e.preventDefault();
    });

    // validation: form elements
    

    // class name
    $('#student-class-name').on('change', function (e) {
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

    $('#student-class-description').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        $(this).removeClass('is-invalid');
        if (e.target.value.length === 0 || e.target.value.length > 256 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
    
});