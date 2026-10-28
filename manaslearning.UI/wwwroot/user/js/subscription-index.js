/**
 * load subscriptions
 * */
function loadSubscriptions() {
    $('.table-subscriptions > tbody').html('');

    let row1 = document.createElement('tr');
    row1.setAttribute('class', 'bg-white');

    // name
    let cell0 = row1.insertCell(0);
    cell0.innerHTML = `<a href="/u/subscriptions/653652d2-4426-4a11-9bb0-76050d7d81bc">ADHD Assessment</a>`;
    // available licenses
    let cell1 = row1.insertCell(1);
    cell1.innerHTML = `2`;
    // assigned licenses
    let cell2 = row1.insertCell(2);
    cell2.innerHTML = `1/3`;
    // account type
    let cell3 = row1.insertCell(3);
    cell3.innerHTML = `Individual`;
    //// invoice
    //let cell5 = row.insertCell(5);
    //cell5.innerHTML = `<i class="bi bi-file-pdf"></i>`;
    //// view
    //let cell6 = row.insertCell(6);
    //cell6.innerHTML = `<i class="bi bi-receipt"></i>`;

    $('.table-subscriptions > tbody').append(row1);

    let row2 = document.createElement('tr');
    row1.setAttribute('class', 'bg-white');

    // name
    let cell20 = row2.insertCell(0);
    cell20.innerHTML = `<a href="/u/subscriptions/653652d2-4426-4a11-9bb0-76050d7d81bc">Autism Assessment</a>`;
    // available licenses
    let cell21 = row2.insertCell(1);
    cell21.innerHTML = `5`;
    // assigned licenses
    let cell22 = row2.insertCell(2);
    cell22.innerHTML = `10/15`;
    // account type
    let cell23 = row2.insertCell(3);
    cell23.innerHTML = `Individual`;

    $('.table-subscriptions > tbody').append(row2);
}

$("document").ready(function () {
    // buy subscriptions
    $('.table-subscriptions').on('click', '.img-buy-subscription', function () {
        //resetStudentForm();
        //$('#addStudentModal').modal({ backdrop: 'static', keyboard: false, show: true });
        loadSubscriptions();
    });
});