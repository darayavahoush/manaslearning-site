$("input.quiz-choice").click(function () {
    var chkChoice = this;
    let elements = $(chkChoice).parent().parent().children(".assessment-question-choice");
    $.each(elements, function (i, e) {
        $(e).removeClass("is-checked");
    });
    if (chkChoice && chkChoice.checked) {
        $(chkChoice).parent("div.assessment-question-choice").addClass("is-checked");
    }
});

$(".submit-test").click(function () {
    let questionCount = $(".assessment-question-title").length;
    let choiceCount = $("input[type='radio']").length / 2;
    if (questionCount !== choiceCount) {
        $(".warning-modal-icon").removeClass("text-warning").addClass("text-danger").html('<i class="ti-na"></i>');
        $(".warning-modal-title").html("Error occurred!!!");
        $(".warning-modal-content").html("Please refresh your page and start your test again.");
        $(".btn-warning-continue").removeClass("btn-warning").addClass("btn-danger");
        $("#warningModal").modal("show");
        return;
    }
    let optionsSelectedCount = $("input[type='radio']:checked").length;
    if (optionsSelectedCount !== questionCount) {
        $(".warning-modal-icon").removeClass("text-danger").addClass("text-warning").html('<i class="ti-alert"></i>');
        $(".warning-modal-title").html("Warning!!!");
        $(".warning-modal-content").html("Please select options for all the questions.");
        $(".btn-warning-continue").removeClass("btn-danger").addClass("btn-warning");
        $("#warningModal").modal("show");
        return;
    }
    let yesCount = $("input.quiz-choice-yes:checked").length;
    let lowerLimit = $(".assessment-container").data("lower-limit");
    let upperLimit = $(".assessment-container").data("upper-limit");
    let defaultMessage = $("#hfDefaultMessage").val();
    let lowerLimitMessage = $("#hfLowerLimitMessage").val();
    let upperLimitMessage = $("#hfUpperLimitMessage").val();

    if ((yesCount === lowerLimit && yesCount === upperLimit) || (yesCount >= lowerLimit && yesCount < upperLimit)) {
        // Your child at risk.
        $(".success-modal-icon").removeClass("text-primary").addClass("text-danger").html('<i class="fa fa-meh-o"></i>');
        $(".success-modal-title").html("Emm!!!");
        $(".success-modal-content").html(lowerLimitMessage);
        $(".btn-success-continue").removeClass("btn-primary").addClass("btn-danger");
        $("#successModal").modal("show");
        return;
    }
    else if (yesCount >= upperLimit) {
        // Your child at risk, consult a professional.
        $(".success-modal-icon").removeClass("text-primary").addClass("text-danger").html('<i class="ti ti-face-sad"></i>');
        $(".success-modal-title").html("Oh no!!!");
        $(".success-modal-content").html(upperLimitMessage);
        $(".btn-success-continue").removeClass("btn-primary").addClass("btn-danger");
        $("#successModal").modal("show");
        return;
    }
    else {
        // Your child does not have any problem.
        $(".success-modal-icon").removeClass("text-danger").addClass("text-primary").html('<i class="ti ti-face-smile"></i>');
        $(".success-modal-title").html("Hurray!!!");
        $(".success-modal-content").html(defaultMessage);
        $(".btn-success-continue").removeClass("btn-danger").addClass("btn-primary");
        $("#successModal").modal("show");
    }
    // TODO: Once result is confirmed by the user, give option to redirect to Home or Free Assessments 
});

$("#successModal").on("hidden.bs.modal", function (e) {
    window.location.pathname = "/free-assessments";
});