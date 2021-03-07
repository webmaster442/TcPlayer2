function HandleError() {
    alert("There was an error handling request.");
}

function DoRequest(url) {
    var xmlRq = new XMLHttpRequest();
    xmlRq.open('GET', window.location.href + url, true);
    xmlRq.onload = function() {
        if (this.status >= 200 && this.status < 400) {
            //everything ok;
            console.log(window.location.href + url);
        } else {
            HandleError();
        }
    };
    xmlRq.onerror = function() {
        HandleError();
    };
    xmlRq.send();
}

function PlayerEvent() {
    var attribute = this.getAttribute("data-url");
    if (attribute !== null && attribute !== undefined) {
        DoRequest(attribute);
    }
};


document.addEventListener("DOMContentLoaded", function(event) {

    var elements = document.getElementsByClassName("mainButton");

    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', PlayerEvent, false);
    }
});