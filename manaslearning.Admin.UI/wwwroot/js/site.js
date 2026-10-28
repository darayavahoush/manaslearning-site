function saveAsFile(fileName, bytesBase64) {
    var link = document.createElement('a');
    link.download = fileName;
    link.href = `data:application/octet-stream;base64,${bytesBase64}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

(function () {
    console.log('self runnable function called.');
    document.addEventListener('readystatechange', event => {
        // When HTML/DOM elements are ready:
        if (event.target.readyState === "interactive") {
            // do nothing for now.
        }

        // When window loaded ( external resources are loaded too- `css`,`src`, etc...) 
        if (event.target.readyState === "complete") {
            initTooltips();
        }
    });
})();

function initTooltips() {
    console.log('initTooltips called.');
    // initialize all tooltips on a page
    let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    let tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });
}