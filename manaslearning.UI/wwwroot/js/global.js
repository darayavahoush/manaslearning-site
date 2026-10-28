/* notification */
//setInterval(function () {
//    let item = sessionStorage.getItem("_hasNotificationShown");
//    if (item) {
//        // do nothing
//    }
//    else if (window.location.pathname !== "/child-academic-problems"){
//        $("#notificationModal").modal("show");
//        sessionStorage.setItem("_hasNotificationShown", "yes");
//    }
//}, 10000);

//$(".btn-notification-yes").click(function () {
//    let _origin = window.location.origin;
//    window.location.assign(_origin + '/child-academic-problems');
//});