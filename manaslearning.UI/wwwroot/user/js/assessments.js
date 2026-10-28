$("document").ready(function () {
    // Agreement
    $("#chkAssessmentAgreement").on("change", function () {
        if (this.checked) {
            $(".select-child-section").show();
        } else {
            $(".select-child-section").hide();
            // TODO: show message
        }
    });

    // Select child for Assessment
    $(".select-child-section select").on("change", function () {
        let _value = this.value;
        if (isNaN(_value)) {
            $(".btn-start-assessmnet").removeClass("btn-primary");
            $(".btn-start-assessmnet").addClass("btn-outline-light");
            $(".btn-start-assessmnet").attr("disabled");
        }
        else {
            $(".btn-start-assessmnet").removeClass("btn-outline-light");
            $(".btn-start-assessmnet").removeAttr("disabled");
            $(".btn-start-assessmnet").addClass("btn-primary");
        }
    });
    $(".btn-start-assessmnet").click(function (e) {
        // TODO: 
        // get assessment id
        let re = /^[a-z0-9-]+$/;
        let id = $(e.currentTarget).data("id");
        if (id.length === 0 || id.length > 50 || !re.exec(id)) {
            swal("Warning!", "Bad request.", "warning");
            return;
        }
        // get student id
        let sid = $.trim($(".select-students").val());
        if (sid.length === 0 || isNaN(sid) || parseInt(sid) <= 0) {
            swal("Warning!", "Bad request.", "warning");
            return;
        }
        // get language id
        let lid = $.trim($(".select-languages").val());
        if (lid.length === 0 || isNaN(lid) || parseInt(lid) <= 0) {
            swal("Warning!", "Bad request.", "warning");
            return;
        }
        window.location = `/u/assessments/adhd/${id}/${sid}/${lid}`;
    });

    /* Paid Assessment */
    $(".submit-assessment").click(function () {
        var isCheckedCount = $(".quiz-choice:checked").length;
        if (isCheckedCount !== 1) {
            $(".alert-container").html('<div class="alert alert-warning alert-dismissible fade show" role="alert">Please select an option.<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button></div>');
            return false;
        }
    });
});