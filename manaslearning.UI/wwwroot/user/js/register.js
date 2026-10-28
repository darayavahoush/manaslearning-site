$('document').ready(function () {

    $('#Input_FirstName').blur(function () {
        setDisplayName();
    });

    $('#Input_LastName').blur(function () {
        setDisplayName();
    });

    function setDisplayName() {
        var firstName = document.getElementById('Input_FirstName').value;
        var lastName = document.getElementById('Input_LastName').value;
        var displayName = firstName == '' ? lastName : firstName+ ' ' + lastName;
        document.getElementById('Input_DisplayName').value = displayName;
    }
});