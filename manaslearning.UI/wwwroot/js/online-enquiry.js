$("#btnSendMessage button.btn-send-message").on("click", function (e) {
    e.preventDefault();
    grecaptcha.ready(function () {
        // TODO: Handle reCaptcha timeout
        grecaptcha.execute(siteKey, { action: 'submit' }) // TODO: pass dynamically assigned variable
            .then(function (token) {
                let $txtName = $("#txtName");
                let $txtEmail = $("#txtEmail");
                let $txtPhoneNumber = $("#txtPhoneNumber");
                let $txtCity = $("#txtCity");
                let $txtAge = $("#txtAge");
                let $txtMessage = $("#txtMessage");

                // reset all the controls
                $txtName.removeClass("is-invalid");
                $txtEmail.removeClass("is-invalid");
                $txtPhoneNumber.removeClass("is-invalid");
                $txtCity.removeClass("is-invalid");
                $txtAge.removeClass("is-invalid");
                $txtMessage.removeClass("is-invalid");

                // input validation
                let name = $txtName.val();
                let email = $txtEmail.val();
                let phoneNumber = $txtPhoneNumber.val();
                let city = $txtCity.val();
                let age = $txtAge.val();
                let message = $txtMessage.val();
                let _requestVerificationToken = $('input:hidden[name="__RequestVerificationToken"]').val();

                // name validation
                name = $.trim(name);
                if (name.length === 0) {
                    $txtName.addClass("is-invalid");
                    $txtName.siblings('.invalid-feedback')[0].innerHTML = "Name is required";
                    $txtName.focus();
                    return;
                }
                else if (name.length > 200) {
                    $txtName.addClass("is-invalid");
                    $txtName.siblings('.invalid-feedback')[0].innerHTML = "Name is invalid";
                    $txtName.focus();
                    return;
                }
                
                // email/phone number validation
                email = $.trim(email).toLowerCase();
                phoneNumber = $.trim(phoneNumber);

                if (email.length === 0 && phoneNumber.length === 0) {
                    $txtEmail.addClass("is-invalid");
                    $txtEmail.siblings('.invalid-feedback')[0].innerHTML = 'Email/PhoneNumber is required.';
                    $txtEmail.focus();
                    return;
                }

                // email validation
                if (email.length > 0 && (email.length > 200 || !email.match(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/g))) {
                    $txtEmail.addClass("is-invalid");
                    $txtEmail.siblings('.invalid-feedback')[0].innerHTML = 'Invalid Email';
                    $txtEmail.focus();
                    return;
                }

                // phonenumber validation                
                if (phoneNumber.length > 50) {
                    $txtPhoneNumber.addClass("is-invalid");
                    $txtPhoneNumber.siblings('.invalid-feedback')[0].innerHTML = 'Invalid Phone Number';
                    $txtPhoneNumber.focus();
                    return;
                }

                // city validation
                city = $.trim(city);
                if (city.length === 0) {
                    $txtCity.addClass("is-invalid");
                    $txtCity.siblings('.invalid-feedback')[0].innerHTML = 'City is required';
                    $txtCity.focus();
                    return;
                }
                else if (city.length > 100) {
                    $txtCity.addClass("is-invalid");
                    $txtCity.siblings('.invalid-feedback')[0].innerHTML = 'City is invalid';
                    $txtCity.focus();
                    return;
                }

                // child age validation
                let _age = Number.parseInt($.trim(age));
                if (isNaN(_age)) {
                    $txtAge.addClass("is-invalid");
                    $txtAge.siblings('.invalid-feedback')[0].innerHTML = 'Age is required';
                    $txtAge.focus();
                    return;
                }
                else if (_age < 3 || _age > 70) {
                    $txtAge.addClass("is-invalid");
                    $txtAge.siblings('.invalid-feedback')[0].innerHTML = 'Age is invalid';
                    $txtAge.focus();
                    return;
                }


                // Get values of checked checkboxes
                let types = $('.checkbox-group input').filter(':checked').map(function () { return this.value; }).get();

                let inputModel = {
                    Name: name.replace(/[^\s*\w.@-]/g, ''),
                    Email: email,
                    Phone: phoneNumber,
                    City: city,
                    ChildAge: _age,
                    Message: message.replace(/[^\s*\w.@-]/g, ''),
                    Token: token,
                    ServiceRequest: { Types: types }
                };

                $.ajax({
                    url: '/api/onlineenquiry',
                    type: 'POST',
                    beforeSend: function () {
                        $("#btnSendMessage button.btn-send-message").attr("disabled", true);
                        $("#btnSendMessage button.btn-send-message").text('Sending...');
                    },
                    contentType: 'application/json; charset=utf-8',
                    headers: { RequestVerificationToken: _requestVerificationToken },
                    data: JSON.stringify(inputModel),
                    dataType: "json",
                    success: function (data, textStatus, jqXHR) {
                        $txtName.val('');
                        $txtEmail.val('');
                        $txtPhoneNumber.val('');
                        $txtCity.val('');
                        $txtAge.val('');
                        $('.checkbox-group input').each(function (i, e) { $(e).prop("checked", false); });
                        $txtMessage.val('');

                        $("div.alert.alert-success.contact-msg").show();
                        setTimeout(function () {
                            $("div.alert.alert-success.contact-msg").hide();
                        }, 10000);

                        // TODO: after success, give additional information to user. What happens next and so on...
                    },
                    error: function (e) {                        
                        $("div.alert.alert-danger.contact-msg").show();
                        setTimeout(function () {
                            $("div.alert.alert-danger.contact-msg").hide();
                        }, 10000);
                    },
                    complete: function () {
                        $("#btnSendMessage button.btn-send-message").removeAttr("disabled");
                        $("#btnSendMessage button.btn-send-message").text('Send Message');
                    }
                });
            });
    });
});