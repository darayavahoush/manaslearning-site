$("#btnSendMessage button.btn").on("click", function (e) {
    e.preventDefault();
    grecaptcha.ready(function () {
        // TODO: Handle reCaptcha timeout
        grecaptcha.execute(siteKey, { action: 'submit' }) // TODO: pass dynamically assigned variable
            .then(function (token) {

                // input validation
                let name = $("#txtContactName").val();
                let email = $("#txtContactEmail").val();
                let subject = $("#txtContactSubject").val();
                let message = $("#txtContactMessage").val();
                let _requestVerificationToken = $('input:hidden[name="__RequestVerificationToken"]').val();

                // reset all the controls
                $("#txtContactName").removeClass("is-invalid");
                $("#txtContactEmail").removeClass("is-invalid");
                $("#txtContactSubject").removeClass("is-invalid");
                $("#txtContactMessage").removeClass("is-invalid");

                // name validation
                name = $.trim(name);
                if (name.length === 0 || name.length > 255) {
                    $("#txtContactName").addClass("is-invalid");
                    $("#txtContactName").focus();
                    return;
                }
                // TODO: show different message for different validation

                // email validation
                email = $.trim(email).toLowerCase();
                if (email.length === 0 || email.length > 255 || !email.match(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/g)) {
                    $("#txtContactEmail").addClass("is-invalid");
                    $("#txtContactEmail").focus();
                    return;
                }
                // TODO: show different message for different validation

                // subject validation
                subject = $.trim(subject);
                if (subject.length === 0 || subject.length > 500) {
                    $("#txtContactSubject").addClass("is-invalid");
                    $("#txtContactSubject").focus();
                    return;
                }
                // TODO: show different message for different validation

                // message validation
                message = $.trim(message);
                if (message.length === 0 || message.length > 1000) {
                    $("#txtContactMessage").addClass("is-invalid");
                    $("#txtContactMessage").focus();
                    return;
                }
                // TODO: show different message for different validation

                let inputModel = {
                    name: name.replace(/[^\s*\w.@-]/g, ''),
                    email: email,
                    subject: subject.replace(/[^\s*\w.@-]/g, ''),
                    message: message.replace(/[^\s*\w.@-]/g, ''),
                    token: token
                };

                $.ajax({
                    url: '/api/contact',
                    type: 'POST',
                    beforeSend: function () {
                        $("#btnSendMessage button.btn").text('Sending...');
                    },
                    contentType: 'application/json; charset=utf-8',
                    headers: { RequestVerificationToken: _requestVerificationToken },
                    data: JSON.stringify(inputModel),
                    dataType: "json",
                    success: function (data, textStatus, jqXHR) {
                        // TODO: clear all fields
                        $("#txtContactName").val("");
                        $("#txtContactEmail").val("");
                        $("#txtContactSubject").val("");
                        $("#txtContactMessage").val("");
                        $("div.alert.alert-success.contact-msg").show();
                        setTimeout(function () {
                            $("div.alert.alert-success.contact-msg").hide();
                        }, 10000);

                        // TODO: after success, give additional information to user. What happens next and so on...
                    },
                    error: function (e) {
                        // TODO: highlight textbox boarder with red color
                        // TODO: show error message
                        console.log(e); // TODO: remove log
                    },
                    complete: function () {
                        $("#btnSendMessage button.btn").text('Send Message');
                    }
                });
            });
    });
});