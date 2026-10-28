/**
 * save student.
 **/
function saveStudent() {
    try {

        disableStudentSaveButton();
        resetErrorMessagesDiv();

        let id = $('#hfRecordId').val();
        let name = $('#student-name').val();
        let parentName = $('#student-parent-name').val();
        let email = $('#student-email').val();
        let phone = $('#student-phone').val();
        let zip = $('#student-zip').val();
        let scs = $('#student-can-speak').val().toString(); // student can speak
        let dob = $('#student-date-of-birth').val();
        let sClass = $('#student-class').val();
        let parentEducation = $('#student-parent-education').val();
        let firstLanguage = $('#student-first-language').val();
        let secondLanguage = $('#student-second-language').val();
        let thirdLanguage = $('#student-third-language').val();
        let motherTongue = $('#student-mother-tongue').val();
        let gender = $('#student-gender').val();
        let schoolType = $('#student-school-type').val();
        let syllabusType = $('#student-syllabus').val();

        // reset
        $('.form-student').find('.form-control').each(function () {
            $(this).removeClass('is-invalid');
        });

        // client side validations
        let re = /^[\w\s-./]+$/;
        // id
        if (id.length === 0 || id.length > 11 || isNaN(id) || parseInt(id) > 99999999999) {
            id = '0';
            // TODO: show error message
        }

        // student name
        if (name.length === 0 || name.length > 50 || !re.exec(name)) {
            $('#student-name').addClass('is-invalid');
            $('#student-name').focus();
            return;
        }

        // parent's name
        if (parentName.length === 0 || parentName.length > 50 || !re.exec(parentName)) {
            $('#student-parent-name').addClass('is-invalid');
            $('#student-parent-name').focus();
            return;
        }

        // email 
        let reEmail = /^[a-z0-9._%+-]{2,}@[a-z0-9.-]{2,}\.[a-z]{2,3}$/;
        if (email.length === 0 || email.length > 256 || !reEmail.exec(email)) {
            $('#student-email').addClass('is-invalid');
            $('#student-email').focus();
            return;
        }

        // phone
        // TODO: update regex to numbers only
        if (phone.length !== 10 || isNaN(phone)) {
            $('#student-phone').addClass('is-invalid');
            $('#student-phone').focus();
            return;
        }

        // zip
        // TODO: update regex to numbers only
        if (zip.length < 6 || zip.length > 10 || isNaN(zip)) {
            $('#student-zip').addClass('is-invalid');
            $('#student-zip').focus();
            return;
        }

        // student can speak
        let regexSCS = /^[0-9,]+$/;
        if (scs.length === 0 || scs.length > 500 || !regexSCS.exec(scs)) {
            $('button.multiselect').addClass(['btn', 'btn-danger']);
            $('button.multiselect').removeClass('custom-select');
            $('button.multiselect').focus();
            return;
        }

        // dob
        // must be between 1/1/1950 and (Current Year - 2).
        let a = moment(dob);
        if (dob.length !== 10 || !moment(dob).isValid() || moment(dob).year() < 1950 || moment(dob).year() > moment().year() - 2) {
            $('#student-date-of-birth').addClass('is-invalid');
            $('#student-date-of-birth').focus();
            return;
        }

        // class
        if (sClass === '-1' || isNaN(sClass) || parseInt(sClass) <= 0) {
            $('#student-class').addClass('is-invalid');
            $('#student-class').focus();
            return;
        }

        // parent's education
        if (parentEducation === '-1' || isNaN(parentEducation) || parseInt(parentEducation) <= 0) {
            $('#student-parent-education').addClass('is-invalid');
            $('#student-parent-education').focus();
            return;
        }

        // first language
        if (firstLanguage === '-1' || isNaN(firstLanguage) || parseInt(firstLanguage) <= 0) {
            $('#student-first-language').addClass('is-invalid');
            $('#student-first-language').focus();
            return;
        }

        // second language
        if (secondLanguage === '-1' || isNaN(secondLanguage) || parseInt(secondLanguage) <= 0) {
            $('#student-second-language').addClass('is-invalid');
            $('#student-second-language').focus();
            return;
        }

        // third language 
        // Note: this is optional field
        //if (thirdLanguage === '-1' || isNaN(thirdLanguage) || parseInt(thirdLanguage) <= 0) {
        //    $('#student-third-language').addClass('is-invalid');
        //    $('#student-third-language').focus();
        //    return;
        //}

        // mother tongue
        if (motherTongue === '-1' || isNaN(motherTongue) || parseInt(motherTongue) <= 0) {
            $('#student-mother-tongue').addClass('is-invalid');
            $('#student-mother-tongue').focus();
            return;
        }

        // gender
        if (gender === '-1' || isNaN(gender) || parseInt(gender) <= 0) {
            $('#student-gender').addClass('is-invalid');
            $('#student-gender').focus();
            return;
        }

        // school type
        if (schoolType === '-1' || isNaN(schoolType) || parseInt(schoolType) <= 0) {
            $('#student-school-type').addClass('is-invalid');
            $('#student-school-type').focus();
            return;
        }

        // syllabus type
        if (syllabusType === '-1' || isNaN(syllabusType) || parseInt(syllabusType) <= 0) {
            $('#student-syllabus').addClass('is-invalid');
            $('#student-syllabus').focus();
            return;
        }


        let request = {
            "recordId": parseInt(id),
            "name": name,
            "parentName": parentName,
            "email": email,
            "phone": phone,
            "zip": zip,
            "studentCanSpeak": scs,
            "dob": dob,
            "studentClass": sClass,
            "parentEducation": parentEducation,
            "firstLanguage": firstLanguage,
            "secondLanguage": secondLanguage,
            "thirdLanguage": thirdLanguage,
            "motherTongue": parseInt(motherTongue),
            "gender": parseInt(gender),
            "schoolType": parseInt(schoolType),
            "syllabusType": parseInt(syllabusType)
        };

        // save student
        $.post('/api/u/student', request, 'json')
            .done(function (data) {
                enableStudentSaveButton();
                // close modal
                $('#addStudentModal').modal('hide');

                // show success message
                $('#statusModal .modal-header').removeClass('bg-primary').removeClass('bg-danger').removeClass('bg-warning').removeClass('bg-info');
                $('#statusModal .modal-title').removeClass('text-white').removeClass('text-dark');
                $('#statusModal .modal-header').addClass('bg-primary');
                $('#statusModal .modal-title').html('<img class="ml-1 mr-2 pb-1" src="/user/icons/checked.svg" width="20" height="20" /> Profile').addClass('text-white');
                $('#statusModal .modal-body').html('Student details saved successfully.');
                $('#statusModal').modal({ backdrop: 'static', keyboard: false, show: true });

                // re-load students
                loadStudents();
            })
            .fail(function (e) {

                enableStudentSaveButton();

                if (e.status === 400) {
                    $('#divErrorMessage').html(e.responseText); // Message: Maximum students limit reached.
                    $('#divErrorMessagesRow').removeClass('d-none');
                }
                else
                    console.log(e);
            });

    } catch (e) {
        console.log(e);
    }
    finally {
        enableStudentSaveButton();
    }
}

function disableStudentSaveButton() {
    $('.btn-save-student').prop('disabled', true);
    $('.btn-save-student').prop('innerHTML', 'Saving...');
}

function enableStudentSaveButton() {
    $('.btn-save-student').prop('disabled', false);
    $('.btn-save-student').prop('innerHTML', 'Save');
}

function resetErrorMessagesDiv() {
    $('#divErrorMessagesRow').addClass('d-none');
    $('#divErrorMessage').html('');
}

/**
 * load students.
 **/
function loadStudents() {
    try {
        $.get('/api/u/student')
            .done(function (data) {
                let students = data;
                let rows = '';
                if (students && students.length > 0) {
                    $('.table-students > tbody').html('');

                    for (var i = 0; i < students.length; i++) {
                        let _item = students[i];
                        let row = document.createElement('tr');
                        row.setAttribute('class', 'bg-white');
                        // name
                        let cell0 = row.insertCell(0);
                        cell0.innerHTML = `<div class="media align-items-center"><div class="media-body"><span class="h6 mb-0">${_item.studentName}</span><span class="text-muted">${_item.studentClass}</span></div></div>`;
                        // parent name
                        let cell1 = row.insertCell(1);
                        cell1.innerHTML = `${_item.parentName}`;
                        // dob
                        let cell2 = row.insertCell(2);
                        cell2.innerHTML = `${moment(_item.dateOfBirth).format("DD-MMM-YYYY")}`;
                        // phone
                        let cell3 = row.insertCell(3);
                        cell3.innerHTML = `${_item.phoneNumber}`;
                        // info
                        let cell4 = row.insertCell(4);
                        cell4.setAttribute('class', 'text-center');
                        cell4.innerHTML = `<img class="img-student-info cursor-pointer" src="/images/document.svg" width="24" height="24" data-id='${_item.studentId}' data-toggle="tooltip" data-placement="top" title="View Student Info" alt="View" />`;
                        // edit
                        let cell5 = row.insertCell(5);
                        cell4.setAttribute('class', 'text-center');
                        cell5.innerHTML = `<img class="img-student-edit cursor-pointer" src="/images/edit.svg" width="24" height="24" data-id='${_item.studentId}' data-toggle="tooltip" data-placement="top" title="Edit Student" alt="Edit" />`;
                        $('.table-students > tbody').append(row);
                    }

                    // wire-up click event for 'Info'
                    $('.table-students > tbody').on('click', 'img.img-student-info', function (e) {
                        getStudentInfo(e);
                    });

                    // wire-up click event for 'Edit'
                    $('.table-students > tbody').on('click', 'img.img-student-edit', function (e) {
                        editStudent(e);
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
 * get student info.
 * @param {any} e
 */
function getStudentInfo(e) {
    try {
        let id = $(e.currentTarget).data('id');
        // TODO: input validation
        $.get(`/api/u/student/info?id=${id}`)
            .done(function (data) {
                let student = data;
                if (student) {
                    $('.table-student-info > tbody').html('' +
                        `<tr><td style="width:8.1rem;">Student Name:</td><td>${student.studentName}</td></tr>` +
                        `<tr><td>Parent's Name:</td><td>${student.parentName}</td></tr>` +
                        `<tr><td>Email</td><td>${student.email}</td></tr>` +
                        `<tr><td>Phone Number</td><td>${student.phoneNumber}</td></tr>` +
                        `<tr><td>PIN Code</td><td>${student.zip}</td></tr>` +
                        `<tr><td>Student can speak:</td><td>${student.languagesCanSpeak}</td></tr>` +
                        `<tr><td>Date fo Birth:</td><td>${moment(student.dateOfBirth).format("DD-MMM-YYYY")}</td></tr>` +
                        `<tr><td>Class:</td><td>${student.studentClass}</td></tr>` +
                        `<tr><td>Parent's Education:</td><td>${student.parentEducation}</td></tr>` +
                        `<tr><td>First Language:</td><td>${student.firstlanguage}</td></tr>` +
                        `<tr><td>Second Language:</td><td>${student.secondLanguage}</td></tr>` +
                        `<tr><td>Third Language:</td><td>${student.thirdLanguage}</td></tr>` +
                        `<tr><td>Mother Tongue:</td><td>${student.motherTongue}</td></tr>` +
                        `<tr><td>Gender:</td><td>${student.gender}</td></tr>` +
                        `<tr><td>School:</td><td>${student.schoolType}</td></tr>` +
                        `<tr><td>Syllabus:</td><td>${student.syllabusType}</td></tr>`);
                }
                $('#studentInfoModal').modal({ backdrop: 'static', keyboard: false, show: true });
            })
            .fail(function (e) {
                console.log(e);});
    } catch (e) {
        console.log(e);
    }
}

/**
 * edit student.
 * @param {any} e
 */
function editStudent(e) {
    try {
        let id = $(e.currentTarget).data('id');
        $('#hfRecordId').val(id);
        // TODO: input validation
        $.get(`/api/u/student/edit?id=${id}`)
            .done(function (data) {
                let student = data;
                if (student) {
                    resetStudentForm();
                    $('#student-name').val(student.studentName);
                    $('#student-parent-name').val(student.parentName);
                    $('#student-email').val(student.email);
                    $('#student-phone').val(student.phoneNumber);
                    $('#student-zip').val(student.zip);
                    $('#student-can-speak').multiselect('select', student.languagesCanSpeak.split(','));
                    $('#student-date-of-birth').val(moment(student.dateOfBirth).format("YYYY-MM-DD"));
                    $('#student-class').val(student.studentClass);
                    $('#student-parent-education').val(student.parentEducation);
                    $('#student-first-language').val(student.firstlanguage);
                    $('#student-second-language').val(student.secondLanguage);
                    $('#student-third-language').val(student.motherTongue);
                    $('#student-mother-tongue').val(student.motherTongue);
                    $('#student-gender').val(student.gender);
                    $('#student-school-type').val(student.schoolType);
                    $('#student-syllabus').val(student.syllabusType);
                }
                $('#addStudentModalLabel').html('Edit Student');
                $('#addStudentModal').modal({ backdrop: 'static', keyboard: false, show: true });
            })
            .fail(function (e) {
                console.log(e);
                $('#addStudentModal').modal('hide');
            });
    } catch (e) {
        console.log(e);
    }
}

/**
 * reset student form.
 **/
function resetStudentForm() {
    enableStudentSaveButton();
    // reset form controls css classes
    $('.form-student').find('.form-control').each(function () {
        $(this).removeClass('is-invalid');
    });

    // name
    $('#student-name').val('');

    // parent name
    $('#student-parent-name').val('');

    // email
    $('#student-email').val('');

    // phone
    $('#student-phone').val('');

    // zip
    $('#student-zip').val('');

    // scs
    $('#student-can-speak').multiselect('deselectAll', false);
    $('button.multiselect').addClass('custom-select');
    $('button.multiselect').removeClass(['btn', 'btn-danger']);

    // dob
    $('#student-date-of-birth').val('');

    // class
    $('#student-class').val('-1');

    // parent education
    $('#student-parent-education').val('-1');

    // first language
    $('#student-first-language').val('-1');

    // second language
    $('#student-second-language').val('-1');

    // third language
    $('#student-third-language').val('-1');

    // mother tongue
    $('#student-mother-tongue').val('-1');

    // gender
    $('#student-gender').val('-1');

    // school type
    $('#student-school-type').val('-1');

    // syllabus type
    $('#student-syllabus').val('-1');

    resetErrorMessagesDiv();
}

$("document").ready(function () {
    $('#student-can-speak').multiselect({
        maxHeight: 200,
        nonSelectedText: 'Select multiple languages',
        numberDisplayed: 4,
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        onDropdownHidden: function (e) {
            let scs = $('#student-can-speak').val().toString();

            //let ok = re.exec(zip);
            // TODO: update regex to accept ','
            if (scs.length === 0 || scs.length > 500) {
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

    // add students
    $('.table-students').on('click', '.img-add-student', function () {
        resetStudentForm();
        $('#addStudentModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    // load students
    loadStudents();

    $('.btn-add-student').click(function () {
        resetStudentForm();
        $('#hfRecordId').val('0');
        $('#addStudentModalLabel').html('Add Student');
        $('#addStudentModal').modal({ backdrop: 'static', keyboard: false, show: true });
    });

    $('#addStudentModal').on('hidden.bs.modal', function (e) { $('#hfRecordId').val('0'); });

    var lastActionTime, difference;
    $('.btn-save-student').click(function (e) {
        if (lastActionTime) {
            difference = e.timeStamp - lastActionTime;
            if (difference >= 1500) { // 1500: Milliseconds              
                saveStudent();
            }
        }
        else {
            lastActionTime = e.timeStamp;
            saveStudent();
        }
        lastActionTime = e.timeStamp;
    });

    $('.btn-save-student').dblclick(function (e) {
        e.preventDefault();
    });

    // validation: form elements
    // student name
    $('#student-name').on('change', function (e) {
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

    // parent's name
    $('#student-parent-name').on('change', function (e) {
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

    // email
    $('#student-email').on('change', function (e) {
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

    // phone
    $('#student-phone').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        // TODO: update regex to allow only numbers
        $(this).removeClass('is-invalid');
        if (e.target.value.length !== 10 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // zip
    $('#student-zip').on('change', function (e) {
        let re = /^[\w\s-./]+$/;
        // TODO: update regex to allow only numbers
        $(this).removeClass('is-invalid');
        if (e.target.value.length < 6 || e.target.value.length > 10 || !re.exec(e.target.value)) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // student can speak
    // already handled.

    // dob
    // must be between 1/1/1950 and (Current Year - 2).
    $('#student-date-of-birth').on('change', function (e) {
        $(this).removeClass('is-invalid');
        let _dob = e.target.value;
        if (_dob.length !== 10 || !moment(_dob.value).isValid() || moment(_dob).year() < 1950 || moment(_dob).year() > moment().year() - 2) {
            $(this).addClass('is-invalid');
            //$(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // TODO: update below onchange events.
    // class
    $('#student-class').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // parent's education
    $('#student-parent-education').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // first language
    $('#student-first-language').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // second language
    $('#student-second-language').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // third language
    //$('#student-third-language').on('change', function (e) {
    //    let _val = e.target.value;
    //    if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
    //        $(this).addClass('is-invalid');
    //        $(this).focus();
    //    }
    //    else {
    //        $(this).removeClass('is-invalid');
    //    }
    //});

    // mother tongue
    $('#student-mother-tongue').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // gender
    $('#student-gender').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // school type
    $('#student-school-type').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });

    // syllabus type
    $('#student-syllabus').on('change', function (e) {
        let _val = e.target.value;
        if (_val === '-1' || isNaN(_val) || parseInt(_val) <= 0) {
            $(this).addClass('is-invalid');
            $(this).focus();
        }
        else {
            $(this).removeClass('is-invalid');
        }
    });
});