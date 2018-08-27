var syncbeforeunload = true;
function checkIsLogout() {
    if (navigator.onLine) {
        $.ajax({
            url: '/User/IsSessionTimeOut',
            type: 'Get',
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                if (data == true) {
                    IsSessionTimeOut1 = 2;
                    syncbeforeunload = false;
                    window.location.href = "/";
                }
                else
                    IsSessionTimeOut1 = 1;
            },
            error: function (xhr, status, error) {
                IsSessionTimeOut1 = 0;
                window.history.forward();
            }
        });
    }
    else {
        IsSessionTimeOut1 = 0;
        window.history.forward();
    }
}

function getOSName() {
    var OSName = "Unknown OS";
    if (navigator.appVersion.indexOf("Win") != -1) OSName = "Windows";
    if (navigator.appVersion.indexOf("Mac") != -1) OSName = "MacOS";
    if (navigator.appVersion.indexOf("X11") != -1) OSName = "UNIX";
    if (navigator.appVersion.indexOf("Linux") != -1) OSName = "Linux";

    return OSName;
}

function CompatibleBrowser() {
    var flag = false;
    var OSName = getOSName();
    var otherOS = false;
    var isFirefox = typeof InstallTrigger !== 'undefined';

    var isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || safari.pushNotification);

    var isChrome = !!window.chrome && !!window.chrome.webstore;

    if (OSName == "Windows") {
        if (isChrome || isFirefox)
            flag = true;
    }
    else if (OSName == "MacOS") {
        if (isChrome || isSafari)
            flag = true;
    }
    else
        otherOS = true;
    
    if (otherOS)
        window.location.href = "OtherOSBrowser.html";
    else if (!flag)
        window.location.href = "browser.html";
}

